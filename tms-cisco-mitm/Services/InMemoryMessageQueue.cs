using System.Threading.Channels;
using tms_cisco_mitm.Models;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class InMemoryMessageQueue : IMessageQueuePublisher
    {
        private readonly Channel<MessageQueueItem> _channel;
        private readonly ILogger<InMemoryMessageQueue> _logger;
        public InMemoryMessageQueue(ILogger<InMemoryMessageQueue> logger)
        {
            _logger = logger;

            // If queue is full, oldest messages are dropped 
            var options = new BoundedChannelOptions(capacity: 10000)
            {
                FullMode = BoundedChannelFullMode.Wait, 
                SingleReader = false, 
                SingleWriter = false  
            };
            _channel = Channel.CreateBounded<MessageQueueItem>(options);
        }

        public async Task PublishAsync(string messageId, string rawXml, string remoteIp)
        {
            try
            {
                var item = new MessageQueueItem
                {
                    MessageId = messageId,
                    RawXml = rawXml,
                    RemoteIp = remoteIp,
                    QueuedAt = DateTime.UtcNow,
                    RetryCount = 0
                };
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await _channel.Writer.WriteAsync(item, cancellationTokenSource.Token);
                _logger.LogDebug("Published message {MessageId} to queue", messageId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Timeout publishing message {MessageId}", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message {MessageId} to queue", messageId);
            }
        }

        public ChannelReader<MessageQueueItem> GetReader()
        {
            return _channel.Reader;
        }

        public async Task CompleteAsync()
        {
            _channel.Writer.Complete();
            await _channel.Reader.Completion;
        }

    }
}
