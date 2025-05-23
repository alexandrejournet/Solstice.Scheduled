﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Solstice.Scheduled.Core;

namespace Solstice.Scheduled.Scheduled;

public abstract class CoreScopedProcessor<TDbContext>(IServiceScopeFactory serviceScopeFactory) : CoreBackgroundService 
    where TDbContext : DbContext
{
    protected readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override async Task ProcessAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await ProcessInScopeAsync(dbContext, cancellationToken);
    }

    public abstract Task ProcessInScopeAsync(TDbContext dbContext, CancellationToken cancellationToken);
}