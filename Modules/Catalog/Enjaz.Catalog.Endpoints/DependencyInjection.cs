using Enjaz.Catalog.Application;
using Enjaz.Catalog.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Catalog.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCatalogApplication();
        services.AddCatalogInfrastructure(configuration);
        services.AddCatalogEndpoints();

        return services;
    }

    public static IServiceCollection AddCatalogEndpoints(this IServiceCollection services)
    {
        return services;
    }
}
