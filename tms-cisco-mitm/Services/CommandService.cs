using System.Collections.Concurrent;
using tms_cisco_mitm.Models.Commands;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class CommandService : ICommandService
    {
        private readonly ConcurrentDictionary<string, DeviceCommand> _commands = new();
        private readonly ILogger<CommandService> _logger;

        public CommandService(ILogger<CommandService> logger)
        {
            _logger = logger;
        }

        public Task<string> QueueCommandAsync(DeviceCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.DeviceSerialNumber))
            {
                _logger.LogError("Cannot queue command without a valid device serial number");
            }

            command.Status = CommandStatus.Queued;
            command.CreatedAt = DateTime.UtcNow;

            if (_commands.TryAdd(command.CommandId, command))
            {
                return Task.FromResult(command.CommandId);
            }

            throw new InvalidOperationException($"Failed to queue command {command.CommandId}");
        }

        public Task<List<DeviceCommand>> GetPendingCommandsAsync(string serialNumber, string? macAddress = null)
        {
            var now = DateTime.UtcNow;
            var commands = _commands.Values
                .Where(c =>
                    (c.DeviceSerialNumber.Equals(serialNumber, StringComparison.OrdinalIgnoreCase) ||
                     (!string.IsNullOrEmpty(macAddress) && c.DeviceMacAddress?.Equals(macAddress, StringComparison.OrdinalIgnoreCase) == true)) &&
                    c.Status == CommandStatus.Queued &&
                    c.DeliveryAttempts < c.MaxDeliveryAttempts &&
                    (c.ExpiresAt == null || c.ExpiresAt > now))
                .OrderByDescending(c => c.Priority)
                .ThenBy(c => c.CreatedAt)
                .ToList();
            return Task.FromResult(commands);
        }

        public Task<bool> MarkCommandDeliveredAsync(string commandId)
        {
            if (_commands.TryGetValue(commandId, out var command))
            {
                command.Status = CommandStatus.Delivered;
                command.DeliveredAt = DateTime.UtcNow;
                command.DeliveryAttempts++;
                return Task.FromResult(true);
            }
            _logger.LogWarning("Attempted to mark unknown command {CommandId} as delivered", commandId);
            return Task.FromResult(false);
        }

        public Task<bool> MarkCommandExecutedAsync(string commandId, string? result = null)
        {
            if (_commands.TryGetValue(commandId, out var command))
            {
                command.Status = CommandStatus.Executed;
                command.ExecutedAt = DateTime.UtcNow;
                command.ExecutionResult = result;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> MarkCommandFailedAsync(string commandId, string errorMessage)
        {
            if (_commands.TryGetValue(commandId, out var command))
            {
                command.Status = CommandStatus.Failed;
                command.ErrorMessage = errorMessage;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<DeviceCommand?> GetCommandAsync(string commandId)
        {
            _commands.TryGetValue(commandId, out var command);
            return Task.FromResult(command);
        }

        public Task<List<DeviceCommand>> GetDeviceCommandsAsync(string serialNumber)
        {
            var commands = _commands.Values
                .Where(c => c.DeviceSerialNumber.Equals(serialNumber, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return Task.FromResult(commands);
        }

        public Task<int> CleanupExpiredCommandsAsync()
        {
            var now = DateTime.UtcNow;
            var expiredCommands = _commands.Values
                .Where(c => c.ExpiresAt != null && c.ExpiresAt <= now && c.Status != CommandStatus.Executed)
                .ToList();

            var count = 0;
            foreach (var command in expiredCommands)
            {
                command.Status = CommandStatus.Expired;
                count++;
            }
            return Task.FromResult(count);
        }

    }
}
