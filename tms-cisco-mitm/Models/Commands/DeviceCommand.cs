namespace tms_cisco_mitm.Models.Commands
{
    public class DeviceCommand
    {
        public string CommandId { get; set; } = Guid.NewGuid().ToString();
        public string DeviceSerialNumber { get; set; } = string.Empty;
        public string? DeviceMacAddress { get; set; }
        public CommandType Type { get; set; }
        public CommandPriority Priority { get; set; } = CommandPriority.Normal;
        public CommandStatus Status { get; set; } = CommandStatus.Pending;
        public string PayloadJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
        public string? ExecutionResult { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RequestedBy { get; set; }
        public int DeliveryAttempts { get; set; } = 0;
        public int MaxDeliveryAttempts { get; set; } = 3;

    }
}
