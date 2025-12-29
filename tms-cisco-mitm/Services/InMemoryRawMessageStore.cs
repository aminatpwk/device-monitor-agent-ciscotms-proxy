using System.Collections.Concurrent;
using tms_cisco_mitm.Models;
using tms_cisco_mitm.Models.Soap;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class InMemoryRawMessageStore : IRawMessageStore
    {
        private readonly ConcurrentDictionary<string, RawMessage> _store = new();
        private readonly ILogger<InMemoryRawMessageStore> _logger;

        public InMemoryRawMessageStore(ILogger<InMemoryRawMessageStore> logger)
        {
            _logger = logger;
        }

        public Task<bool> StoreRawMessageAsync(string messageId, string rawXml, string remoteIp)
        {
            try
            {
                var message = new RawMessage
                {
                    MessageId = messageId,
                    RawXml = rawXml,
                    RemoteIp = remoteIp,
                    ReceivedAt = DateTime.UtcNow
                };

                var stored = _store.TryAdd(messageId, message);

                if (stored)
                {
                    _logger.LogDebug("Stored raw message {MessageId}", messageId);
                }
                else
                {
                    _logger.LogWarning("Failed to store message {MessageId} - duplicate ID", messageId);
                }

                return Task.FromResult(stored);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing message {MessageId}", messageId);
                return Task.FromResult(false);
            }
        }

        public Task<RawMessage?> GetRawMessageAsync(string messageId)
        {
            _store.TryGetValue(messageId, out var message);
            return Task.FromResult(message);
        }

    }
}
