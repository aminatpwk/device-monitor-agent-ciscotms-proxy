using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Worker
{
    public class CommandCleanupWorker : BackgroundService
    {
        private readonly ICommandService _commandService;
        private readonly ILogger<CommandCleanupWorker> _logger;
        private readonly TimeSpan _cleanupInterval;

        public CommandCleanupWorker(ICommandService commandService, IConfiguration configuration, ILogger<CommandCleanupWorker> logger)
        {
            _commandService = commandService;
            _logger = logger;
            _cleanupInterval = TimeSpan.FromMinutes(
                configuration.GetValue<int>("DeviceManagement:CommandCleanupIntervalMinutes", 15)
            );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    var expiredCount = await _commandService.CleanupExpiredCommandsAsync();
                    if (expiredCount > 0)
                    {
                        _logger.LogInformation("Cleaned up {Count} expired commands", expiredCount);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during command cleanup");
                }
            }

            _logger.LogInformation("Command cleanup worker stopped");
        }
    }
}
