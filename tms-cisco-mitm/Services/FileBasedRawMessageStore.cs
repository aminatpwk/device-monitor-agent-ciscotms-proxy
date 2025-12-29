using tms_cisco_mitm.Models;
using tms_cisco_mitm.Models.Soap;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class FileBasedRawMessageStore : IRawMessageStore
    {
        private readonly string _storageDirectory;
        private readonly ILogger<FileBasedRawMessageStore> _logger;

        public FileBasedRawMessageStore(IConfiguration configuration, ILogger<FileBasedRawMessageStore> logger)
        {
            _storageDirectory = configuration["RawMessageStore:Directory"] ?? Path.Combine(Directory.GetCurrentDirectory(), "raw-messages");
            _logger = logger;
            Directory.CreateDirectory(_storageDirectory);
        }

        public async Task<bool> StoreRawMessageAsync(string messageId, string rawXml, string remoteIp)
        {
            try
            {
                var filePath = Path.Combine(_storageDirectory, $"{messageId}.xml");
                var metadataPath = Path.Combine(_storageDirectory, $"{messageId}.meta.json");
                await File.WriteAllTextAsync(filePath, rawXml);

                var metadata = new
                {
                    MessageId = messageId,
                    RemoteIp = remoteIp,
                    ReceivedAt = DateTime.UtcNow,
                    FilePath = filePath
                };
                await File.WriteAllTextAsync(metadataPath, System.Text.Json.JsonSerializer.Serialize(metadata));

                _logger.LogDebug("Stored raw message {MessageId} to {FilePath}", messageId, filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store message {MessageId} to file system", messageId);
                return false;
            }
        }

        public async Task<RawMessage?> GetRawMessageAsync(string messageId)
        {
            try
            {
                var filePath = Path.Combine(_storageDirectory, $"{messageId}.xml");
                var metadataPath = Path.Combine(_storageDirectory, $"{messageId}.meta.json");

                if (!File.Exists(filePath))
                {
                    return null;
                }

                var rawXml = await File.ReadAllTextAsync(filePath);
                var metadataJson = await File.ReadAllTextAsync(metadataPath);
                var metadata = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(metadataJson);

                return new RawMessage
                {
                    MessageId = messageId,
                    RawXml = rawXml,
                    RemoteIp = metadata?["RemoteIp"]?.ToString() ?? "unknown",
                    ReceivedAt = metadata != null && metadata.TryGetValue("ReceivedAt", out var dt)
                        ? DateTime.Parse(dt.ToString()!)
                        : DateTime.MinValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve message {MessageId}", messageId);
                return null;
            }
        }

    }
}
