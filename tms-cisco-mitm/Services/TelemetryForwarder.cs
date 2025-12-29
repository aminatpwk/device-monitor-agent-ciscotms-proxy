using System.Text;
using System.Text.Json;
using tms_cisco_mitm.Models;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class TelemetryForwarder : ITelemetryForwarder
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TelemetryForwarder> _logger;
        private readonly string _c3TelemetryEndpoint;
        private readonly bool _enabled;

        public TelemetryForwarder(HttpClient httpClient, IConfiguration configuration, ILogger<TelemetryForwarder> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _c3TelemetryEndpoint = configuration["C3:TelemetryEndpoint"];
            _enabled = configuration.GetValue<bool>("C3:ForwardingEnabled", false);
        }

        public async Task<bool> ForwardAsync(ParsedSoapMessage message, string messageId, string remoteIp)
        {
            if (!_enabled)
            {
                _logger.LogDebug("Telemetry forwarding is disabled... skipping message {MessageId}", messageId);
                return true;
            }

            try
            {
                var telemetryData = TransformToC3Format(message, messageId, remoteIp);
                var json = JsonSerializer.Serialize(telemetryData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogDebug("Forwarding telemetry for message {MessageId} to {Endpoint}", messageId, _c3TelemetryEndpoint);

                var response = await _httpClient.PostAsync(_c3TelemetryEndpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully forwarded telemetry for message {MessageId}", messageId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to forward telemetry for message {MessageId}. Status: {StatusCode}", messageId, response.StatusCode);
                    return false;
                }

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error forwarding telemetry for message {MessageId}", messageId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding telemetry for message {MessageId}", messageId);
                return false;
            }
        }

        private object TransformToC3Format(ParsedSoapMessage message, string messageId, string remoteIp)
        {
            var identification = message.Identification;

            return new
            {
                MessageId = messageId,
                Timestamp = message.ParsedAt,
                Source = "cisco-tms-proxy",
                Device = new
                {
                    SerialNumber = identification?.SerialNumber,
                    MacAddress = identification?.MACAddress,
                    IpAddress = identification?.IPAddress ?? remoteIp,
                    IpAddressV6 = identification?.IPAddressV6,
                    NatAddress = identification?.NATAddress,
                    SystemName = identification?.SystemName,
                    ProductType = identification?.ProductType,
                    ProductId = identification?.ProductId,
                    SystemType = identification?.SystemType,
                    SoftwareVersion = identification?.SWVersion
                },
                Event = new
                {
                    Type = message.Event,
                    ReceivedAt = message.ParsedAt
                },
                Metadata = new
                {
                    RemoteIp = remoteIp,
                    Protocol = "SOAP/XML"
                }
            };
        }
    }
}
