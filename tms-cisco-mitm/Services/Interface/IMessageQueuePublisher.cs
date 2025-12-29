namespace tms_cisco_mitm.Services.Interface
{
    public interface IMessageQueuePublisher
    {
        Task PublishAsync(string messageId, string rawXml, string remoteIp);
    }
}
