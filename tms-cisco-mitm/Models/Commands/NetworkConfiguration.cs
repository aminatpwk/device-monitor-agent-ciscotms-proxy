namespace tms_cisco_mitm.Models.Commands
{
    public class NetworkConfiguration
    {
        public string? IpAddress { get; set; }
        public string? SubnetMask { get; set; }
        public string? Gateway { get; set; }
        public string? DnsServer { get; set; }
    }
}
