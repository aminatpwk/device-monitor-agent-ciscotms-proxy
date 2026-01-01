using tms_cisco_mitm.Models.Commands;

namespace tms_cisco_mitm.Models.Requests
{
    public class TriggerUpgradeRequest
    {
        public string FileUrl { get; set; } = string.Empty;
        public string TargetVersion { get; set; } = string.Empty;
        public bool RebootAfterUpgrade { get; set; } = true;
        public string? CRC { get; set; }
        public int Size { get; set; }
        public CommandPriority? Priority { get; set; }
        public string? RequestedBy { get; set; }
    }
}
