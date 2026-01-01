using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Models.Commands
{
    public class DeviceInfo
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
        public string? IpAddress { get; set; }
        public string? SystemName { get; set; }
        public string? ProductType { get; set; }
        public string? SoftwareVersion { get; set; }
        public DateTime LastContactAt { get; set; }
        public DateTime FirstSeenAt { get; set; }
        public int HeartbeatInterval { get; set; } 
        public bool IsActive { get; set; } = true;
        public int PendingCommandCount { get; set; }
    }
}
