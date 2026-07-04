using Enjaz.Maps.Application;
using Enjaz.Maps.Application.Maps;
using Enjaz.Maps.Endpoints.Realtime;
using Enjaz.Maps.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Maps.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddMapsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMapsApplication();
        services.AddMapsInfrastructure(configuration);
        services.AddMapsEndpoints();

        return services;
    }

    public static IServiceCollection AddMapsEndpoints(this IServiceCollection services)
    {
        services.AddScoped<ITrackingEventPublisher, SignalRTrackingEventPublisher>();

        return services;
    }
}
