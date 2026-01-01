using System.Collections.Concurrent;
using tms_cisco_mitm.Models.Commands;
using tms_cisco_mitm.Models.Soap.Management;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class DeviceRegistry : IDeviceRegistry
    {
        private readonly ConcurrentDictionary<string, DeviceInfo> _devices = new();
        private readonly ILogger<DeviceRegistry> _logger;
        private readonly IConfiguration _configuration;
        private readonly int _defaultHeartbeatInterval;

        public DeviceRegistry(ILogger<DeviceRegistry> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _defaultHeartbeatInterval = configuration.GetValue<int>("DeviceManagement:DefaultHeartbeatInterval", 3600);
        }

        public Task RegisterDeviceAsync(Identification identification, string remoteIp)
        {
            if (string.IsNullOrWhiteSpace(identification?.SerialNumber))
            {
                _logger.LogWarning("Attempted to register device without serial number");
                return Task.CompletedTask;
            }

            var serialNumber = identification.SerialNumber;
            var now = DateTime.UtcNow;

            var device = _devices.AddOrUpdate(
                serialNumber,
                _ => new DeviceInfo
                {
                    SerialNumber = serialNumber,
                    MacAddress = identification.MACAddress,
                    IpAddress = identification.IPAddress ?? remoteIp,
                    SystemName = identification.SystemName,
                    ProductType = identification.ProductType,
                    SoftwareVersion = identification.SWVersion,
                    FirstSeenAt = now,
                    LastContactAt = now,
                    HeartbeatInterval = _defaultHeartbeatInterval,
                    IsActive = true
                },
                (_, existing) =>
                {
                    existing.MacAddress = identification.MACAddress ?? existing.MacAddress;
                    existing.IpAddress = identification.IPAddress ?? remoteIp;
                    existing.SystemName = identification.SystemName ?? existing.SystemName;
                    existing.ProductType = identification.ProductType ?? existing.ProductType;
                    existing.SoftwareVersion = identification.SWVersion ?? existing.SoftwareVersion;
                    existing.LastContactAt = now;
                    existing.IsActive = true;
                    return existing;
                }
            );

            return Task.CompletedTask;
        }

        public Task<DeviceInfo?> GetDeviceAsync(string serialNumber)
        {
            _devices.TryGetValue(serialNumber, out var device);
            return Task.FromResult(device);
        }

        public Task<List<DeviceInfo>> GetActiveDevicesAsync()
        {
            var devices = _devices.Values
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.LastContactAt)
                .ToList();

            return Task.FromResult(devices);
        }

        public Task<int> GetHeartbeatIntervalAsync(string serialNumber)
        {
            if (_devices.TryGetValue(serialNumber, out var device))
            {
                return Task.FromResult(device.HeartbeatInterval);
            }

            return Task.FromResult(_defaultHeartbeatInterval);
        }

        public Task UpdateHeartbeatIntervalAsync(string serialNumber, int intervalSeconds)
        {
            if (_devices.TryGetValue(serialNumber, out var device))
            {
                device.HeartbeatInterval = intervalSeconds;
            }

            return Task.CompletedTask;
        }

        public Task<int> MarkInactiveDevicesAsync(TimeSpan inactiveThreshold)
        {
            var cutoff = DateTime.UtcNow - inactiveThreshold;
            var count = 0;

            foreach (var device in _devices.Values.Where(d => d.IsActive && d.LastContactAt < cutoff))
            {
                device.IsActive = false;
                count++;
            }

            return Task.FromResult(count);
        }
    }
}
