using Microsoft.Extensions.Hosting;

namespace Solstice.Scheduled.Core;

public abstract class CoreBackgroundService : IHostedService
{
    private Task _executingTask;
    private CancellationTokenSource _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_cts.Token);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            _cts.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    protected virtual async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            await ProcessAsync(cancellationToken);
            await Task.Delay(5000, cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested);
    }

    protected abstract Task ProcessAsync(CancellationToken cancellationToken);
}