using Microsoft.Extensions.Logging;
using Solstice.Scheduled.Core;

namespace Solstice.Scheduled.Queue;

public sealed class CoreQueuedBackgroundService(
    IBackgroundTaskQueue taskQueue,
    ILogger<CoreQueuedBackgroundService> logger)
    : CoreBackgroundService
{
    private readonly Task? _executingTask;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            $"{nameof(CoreQueuedBackgroundService)} is running.");

        return base.ExecuteAsync(stoppingToken);
    }

    public new async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            $"{nameof(CoreQueuedBackgroundService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }

    protected override async Task ProcessAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("QueueService: Process task");
            QueueItem queueItem =
                await taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await queueItem.WorkItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
                logger.LogWarning("OperationCanceledException: Error occurred executing task work item.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }
}