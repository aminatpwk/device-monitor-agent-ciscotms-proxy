using tms_cisco_mitm.Models;

namespace tms_cisco_mitm.Services.Interface
{
    public interface ITelemetryForwarder
    {
        Task<bool> ForwardAsync(ParsedSoapMessage message, string messageId, string remoteIp);
    }
}
