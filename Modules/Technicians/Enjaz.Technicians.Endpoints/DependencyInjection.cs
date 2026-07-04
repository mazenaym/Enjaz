using Enjaz.Technicians.Application;
using Enjaz.Technicians.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Technicians.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddTechniciansModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTechniciansApplication();
        services.AddTechniciansInfrastructure(configuration);
        services.AddTechniciansEndpoints();

        return services;
    }

    public static IServiceCollection AddTechniciansEndpoints(this IServiceCollection services)
    {
        return services;
    }
}
