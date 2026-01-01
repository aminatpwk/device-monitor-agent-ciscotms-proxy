using tms_cisco_mitm.Models.Commands;

namespace tms_cisco_mitm.Services.Interface
{
    public interface ICommandService
    {
        Task<string> QueueCommandAsync(DeviceCommand command);
        Task<List<DeviceCommand>> GetPendingCommandsAsync(string serialNumber, string? macAddress = null);
        Task<bool> MarkCommandDeliveredAsync(string commandId);
        Task<bool> MarkCommandExecutedAsync(string commandId, string? result = null);
        Task<bool> MarkCommandFailedAsync(string commandId, string errorMessage);
        Task<DeviceCommand?> GetCommandAsync(string commandId);
        Task<List<DeviceCommand>> GetDeviceCommandsAsync(string serialNumber);
        Task<int> CleanupExpiredCommandsAsync();

    }
}
