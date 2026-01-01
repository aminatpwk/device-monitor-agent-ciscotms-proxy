using tms_cisco_mitm.Models.Commands;

namespace tms_cisco_mitm.Models.Requests
{
    public class SubmitCommandRequest
    {
        public string DeviceSerialNumber { get; set; } = string.Empty;
        public CommandType Type { get; set; }
        public CommandPriority Priority { get; set; } = CommandPriority.Normal;
        public object Payload { get; set; } = new();
        public string? RequestedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
