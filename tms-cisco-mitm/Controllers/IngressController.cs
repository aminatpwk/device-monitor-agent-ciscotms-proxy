using Microsoft.AspNetCore.Mvc;
using System.Text;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Controllers
{
    [ApiController]
    [Route("tms")]
    public class IngressController : ControllerBase
    {
        private readonly IRawMessageStore _messageStore;
        private readonly IMessageQueuePublisher _queuePublisher;
        private readonly ICommandService _commandService;
        private readonly IDeviceRegistry _deviceRegistry;
        private readonly ISoapResponseBuilder _responseBuilder;
        private readonly ISoapParserService _parser;
        private readonly ILogger<IngressController> _logger;

        public IngressController(IRawMessageStore messageStore, IMessageQueuePublisher queuePublisher, ICommandService commandService, IDeviceRegistry deviceRegistry, ISoapResponseBuilder responseBuilder, ISoapParserService parser, ILogger<IngressController> logger)
        {
            _messageStore = messageStore;
            _queuePublisher = queuePublisher;
            _commandService = commandService;
            _deviceRegistry = deviceRegistry;
            _responseBuilder = responseBuilder;
            _parser = parser;
            _logger = logger;
        }

        [HttpPost("PostEvent")]
        [Consumes("text/xml", "application/soap+xml")]
        public async Task<IActionResult> PostEvent()
        {
            var messageId = Guid.NewGuid().ToString();
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            try
            {
                string rawXml;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: false))
                {
                    rawXml = await reader.ReadToEndAsync();
                }
                if (string.IsNullOrWhiteSpace(rawXml))
                {
                    _logger.LogWarning("Empty payload received from {RemoteIp}", remoteIp);
                    return BadRequest(CreateFailureXml("Empty payload"));
                }

                var parsed = await _parser.ParseAsync(rawXml);
                if (parsed?.Identification == null)
                {
                    _logger.LogWarning("Could not parse device identification from {RemoteIp}", remoteIp);
                    return Ok(CreateSuccessXml());
                }
                await _deviceRegistry.RegisterDeviceAsync(parsed.Identification, remoteIp);

                var stored = await _messageStore.StoreRawMessageAsync(messageId, rawXml, remoteIp);
                if (!stored)
                {
                    _logger.LogError("Failed to store raw message {MessageId} from {RemoteIp}", messageId, remoteIp);
                    return StatusCode(500, CreateFailureXml("Storage failure"));
                }

                await _queuePublisher.PublishAsync(messageId, rawXml, remoteIp);

                var serialNumber = parsed.Identification.SerialNumber;
                var macAddress = parsed.Identification.MACAddress;
                var pendingCommands = await _commandService.GetPendingCommandsAsync(serialNumber, macAddress);
                foreach (var cmd in pendingCommands)
                {
                    await _commandService.MarkCommandDeliveredAsync(cmd.CommandId);
                }

                var heartbeatInterval = await _deviceRegistry.GetHeartbeatIntervalAsync(serialNumber);
                var responseXml = _responseBuilder.BuildPostEventResponse(pendingCommands, heartbeatInterval);
                return Content(responseXml, "text/xml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request from {RemoteIp}, MessageId: {MessageId}", remoteIp, messageId);
                return StatusCode(500, CreateFailureXml("Internal error"));
            }
        }

        [HttpPost("PostDocument")]
        [Consumes("text/xml")]
        public async Task<IActionResult> PostDocument()
        {
            var messageId = Guid.NewGuid().ToString();
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            try
            {
                string rawXml;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: false))
                {
                    rawXml = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(rawXml))
                {
                    return BadRequest(CreateFailureXml("Empty payload"));
                }

                await _messageStore.StoreRawMessageAsync(messageId, rawXml, remoteIp);
                await _queuePublisher.PublishAsync(messageId, rawXml, remoteIp);

                return Ok(CreateSuccessXml());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PostDocument from {RemoteIp}", remoteIp);
                return StatusCode(500, CreateFailureXml("Internal error"));
            }
        }

        private static string CreateSuccessXml()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?><Success/>";
        }

        private static string CreateFailureXml(string reason)
        {
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?><Failure><Reason>{reason}</Reason></Failure>";
        }

    }
}
