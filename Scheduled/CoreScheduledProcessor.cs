using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;

namespace Solstice.Scheduled.Scheduled;

public abstract class CoreScheduledProcessor<TDbContext> : CoreScopedProcessor<TDbContext> 
    where TDbContext : DbContext
{
    private readonly CrontabSchedule _schedule;
    private DateTime _nextRun;

    protected abstract string Schedule { get; }

    protected CoreScheduledProcessor(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {
        _schedule = CrontabSchedule.Parse(Schedule);
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            var now = DateTime.Now;
            var nextrun = _schedule.GetNextOccurrence(now);
            if (now > _nextRun)
            {
                await ProcessAsync(stoppingToken);
                _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            }
            await Task.Delay(5000, stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested);
    }
}