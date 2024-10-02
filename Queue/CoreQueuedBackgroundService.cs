using Microsoft.Extensions.Logging;
using Radiant.Service.Scheduled.Core;
using Scrap.Applications.Background.Queue;

namespace Radiant.Service.Scheduled.Queue;

public sealed class CoreQueuedBackgroundService : CoreBackgroundService
{
    private readonly Task _executingTask;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<CoreQueuedBackgroundService> _logger;

    public CoreQueuedBackgroundService(
        IBackgroundTaskQueue taskQueue,
        ILogger<CoreQueuedBackgroundService> logger) =>
        (_taskQueue, _logger) = (taskQueue, logger);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(CoreQueuedBackgroundService)} is running.");

        return base.ExecuteAsync(stoppingToken);
    }

    public async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(CoreQueuedBackgroundService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }

    protected override async Task ProcessAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("QueueService: Process task");
            QueueItem queueItem =
                await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await queueItem.WorkItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
                _logger.LogWarning("OperationCanceledException: Error occurred executing task work item.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }
}