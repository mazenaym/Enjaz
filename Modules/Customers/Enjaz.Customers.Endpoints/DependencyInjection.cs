using Enjaz.Customers.Application;
using Enjaz.Customers.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Customers.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCustomersApplication();
        services.AddCustomersInfrastructure(configuration);
        services.AddCustomersEndpoints();

        return services;
    }

    public static IServiceCollection AddCustomersEndpoints(this IServiceCollection services)
    {
        return services;
    }
}
