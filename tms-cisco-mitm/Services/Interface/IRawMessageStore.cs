using tms_cisco_mitm.Models;

namespace tms_cisco_mitm.Services.Interface
{
    public interface IRawMessageStore
    {
        Task<bool> StoreRawMessageAsync(string messageId, string rawXml, string remoteIp);
        Task<RawMessage?> GetRawMessageAsync(string messageId);
    }
}
