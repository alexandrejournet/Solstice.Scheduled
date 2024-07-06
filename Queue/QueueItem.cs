namespace Scrap.Applications.Background.Queue
{
    public class QueueItem
    {
        public Guid Guid { get; init; }
        public Func<CancellationToken, Task>? WorkItem { get; init; }
    }
}

