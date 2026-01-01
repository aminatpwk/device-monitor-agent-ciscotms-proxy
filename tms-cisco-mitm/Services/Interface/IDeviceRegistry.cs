using tms_cisco_mitm.Models.Commands;
using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Services.Interface
{
    public interface IDeviceRegistry
    {
        Task RegisterDeviceAsync(Identification identification, string remoteIp);
        Task<DeviceInfo?> GetDeviceAsync(string serialNumber);
        Task<List<DeviceInfo>> GetActiveDevicesAsync();
        Task<int> GetHeartbeatIntervalAsync(string serialNumber);
        Task UpdateHeartbeatIntervalAsync(string serialNumber, int intervalSeconds);
        Task<int> MarkInactiveDevicesAsync(TimeSpan inactiveThreshold);
    }
}
