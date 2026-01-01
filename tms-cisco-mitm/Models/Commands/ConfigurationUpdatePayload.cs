namespace tms_cisco_mitm.Models.Commands
{
    public class ConfigurationUpdatePayload
    {
        public string? SystemName { get; set; }
        public SipConfiguration? Sip { get; set; }
        public GatekeeperConfiguration? Gatekeeper { get; set; }
        public NetworkConfiguration? Network { get; set; }
        public Dictionary<string, string>? CustomSettings { get; set; }
    }
}
