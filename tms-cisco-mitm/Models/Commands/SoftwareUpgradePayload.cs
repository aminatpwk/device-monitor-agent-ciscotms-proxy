using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Models.Commands
{
    public class SoftwareUpgradePayload
    {
        public string FileUrl { get; set; } = string.Empty;
        public string? CRC { get; set; }
        public int Size { get; set; }
        public bool RebootAfterUpgrade { get; set; } = true;
        public string? ReleaseKey { get; set; }
        public string? Comment { get; set; }
        public string? TargetVersion { get; set; }
    }
}
