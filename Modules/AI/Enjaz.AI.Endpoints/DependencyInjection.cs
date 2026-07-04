using Enjaz.AI.Application;
using Enjaz.AI.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.AI.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddAiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAiApplication();
        services.AddAiInfrastructure(configuration);
        services.AddAiEndpoints();

        return services;
    }

    public static IServiceCollection AddAiEndpoints(this IServiceCollection services)
    {
        return services;
    }
}
