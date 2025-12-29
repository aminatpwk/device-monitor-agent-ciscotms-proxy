using tms_cisco_mitm.Services;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Worker
{
    public class MessageProcessorWorker : BackgroundService
    {
        private readonly InMemoryMessageQueue _queue;
        private readonly ISoapParserService _parser;
        private readonly ITelemetryForwarder _forwarder;
        private readonly ILogger<MessageProcessorWorker> _logger;
        private readonly int _workerCount;

        public MessageProcessorWorker(InMemoryMessageQueue queue, ISoapParserService parser, ITelemetryForwarder forwarder, IConfiguration configuration, ILogger<MessageProcessorWorker> logger)
        {
            _queue = queue;
            _parser = parser;
            _forwarder = forwarder;
            _logger = logger;
            _workerCount = configuration.GetValue<int>("MessageProcessor:WorkerCount", 3);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var workers = new List<Task>();
            for (int i = 0; i < _workerCount; i++)
            {
                var workerId = i + 1;
                workers.Add(ProcessMessagesAsync(workerId, stoppingToken));
            }
            await Task.WhenAll(workers);
        }

        private async Task ProcessMessagesAsync(int workerId, CancellationToken stoppingToken)
        {
            var reader = _queue.GetReader();
            await foreach (var item in reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var parsed = await _parser.ParseAsync(item.RawXml);
                    if (parsed == null)
                    {
                        _logger.LogWarning("Worker {WorkerId} failed to parse message {MessageId}", workerId, item.MessageId);
                        continue;
                    }

                    var forwarded = await _forwarder.ForwardAsync(parsed, item.MessageId, item.RemoteIp);
                    if (forwarded)
                    {
                        _logger.LogInformation(
                            "Worker {WorkerId} successfully processed message {MessageId} from device {DeviceName}",
                            workerId, item.MessageId, parsed.Identification?.SystemName ?? "unknown"
                        );
                    }
                    else
                    {
                        _logger.LogWarning("Worker {WorkerId} failed to forward message {MessageId}", workerId, item.MessageId);
                        // TODO: Implement retry logic 
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker {WorkerId} error processing message {MessageId}", workerId, item.MessageId);
                }
            }
        }

    }
}
