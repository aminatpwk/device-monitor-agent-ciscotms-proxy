using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Worker
{
    public class DeviceMonitorWorker : BackgroundService
    {
        private readonly IDeviceRegistry _deviceRegistry;
        private readonly ILogger<DeviceMonitorWorker> _logger;
        private readonly TimeSpan _checkInterval;
        private readonly TimeSpan _inactiveThreshold;

        public DeviceMonitorWorker(IDeviceRegistry deviceRegistry, IConfiguration configuration, ILogger<DeviceMonitorWorker> logger)
        {
            _deviceRegistry = deviceRegistry;
            _logger = logger;
            _checkInterval = TimeSpan.FromMinutes(
                configuration.GetValue<int>("DeviceManagement:MonitorIntervalMinutes", 10)
            );
            _inactiveThreshold = TimeSpan.FromHours(
                configuration.GetValue<int>("DeviceManagement:InactiveThresholdHours", 6)
            );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                    var inactiveCount = await _deviceRegistry.MarkInactiveDevicesAsync(_inactiveThreshold);
                    if (inactiveCount > 0)
                    {
                        _logger.LogWarning("Marked {Count} devices as inactive", inactiveCount);
                    }
                    var activeDevices = await _deviceRegistry.GetActiveDevicesAsync();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during device monitoring");
                }
            }

            _logger.LogInformation("Device monitor stopped");
        }
    }
}
