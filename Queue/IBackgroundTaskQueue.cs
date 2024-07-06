﻿namespace Scrap.Applications.Background.Queue
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(
        QueueItem workItem);

        ValueTask<QueueItem> DequeueAsync(
            CancellationToken cancellationToken);

        void ClearChannel();
    }
}