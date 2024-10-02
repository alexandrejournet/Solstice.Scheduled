using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrap.Applications.Background.Queue;

namespace Radiant.Service.Scheduled.Injections;

public static class ScheduledInjections
{
    /// <summary>
    /// Injector to add a queue to the services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="capacity">Queue capacity</param>
    public static void AddQueue(this IServiceCollection services, int capacity = 100_000)
    {
        services.AddSingleton<IBackgroundTaskQueue>(_ => new DefaultBackgroundTaskQueue(capacity));
    }
    
    /// <summary>
    /// Injector to add hosted services to the services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="types"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>services.AddCronServices(typeof(HostedService1), typeof(HostedService2), ...)</remarks>
    public static void AddCronServices(this IServiceCollection services, params Type[] types)
    {
        foreach (var type in types)
        {
            // Ensure the type implements IHostedService
            if (!typeof(IHostedService).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type {type.FullName} does not implement IHostedService.");
            }

            // Get the generic method definition of AddHostedService
            var method = typeof(ServiceCollectionHostedServiceExtensions)
                .GetMethods()
                .First(m => m is { Name: "AddHostedService", IsGenericMethod: true });

            // Make the method generic with the current type
            var genericMethod = method.MakeGenericMethod(type);

            // Invoke the method on the services collection
            genericMethod.Invoke(null, [services]);
        }
    }
}