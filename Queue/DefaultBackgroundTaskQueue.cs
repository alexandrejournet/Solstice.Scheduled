using System.Threading.Channels;

namespace Solstice.Scheduled.Queue;

public sealed class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<QueueItem> _queue;

    public DefaultBackgroundTaskQueue(
        int capacity)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<QueueItem>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(
        QueueItem workItem)
    {
        Console.WriteLine($"Queue item {workItem.Guid}");
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<QueueItem> DequeueAsync(
        CancellationToken cancellationToken)
    {

        if (_queue.Reader.TryRead(out QueueItem workItem))
        {
            Console.WriteLine($"=> Dequeue item {workItem.Guid}");
            return workItem;
        }

        return null;

    }

    public void ClearChannel()
    {
        _queue.Reader.ReadAllAsync();
    }
}