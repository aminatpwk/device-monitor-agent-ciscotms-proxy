namespace tms_cisco_mitm.Models
{
    public class MessageQueueItem
    {
        public string MessageId { get; set; } = string.Empty;
        public string RawXml { get; set; } = string.Empty;
        public string RemoteIp { get; set; } = string.Empty;
        public DateTime QueuedAt { get; set; }
        public int RetryCount { get; set; }

    }
}
