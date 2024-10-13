namespace Solstice.Scheduled.Queue;

public class QueueItem
{
    public Guid Guid { get; init; }
    public Func<CancellationToken, Task>? WorkItem { get; init; }
}