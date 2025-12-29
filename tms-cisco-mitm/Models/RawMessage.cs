namespace tms_cisco_mitm.Models
{
    public class RawMessage
    {
        public string MessageId { get; set; } = string.Empty;
        public string RawXml { get; set; } = string.Empty;
        public string RemoteIp { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
    }
}
