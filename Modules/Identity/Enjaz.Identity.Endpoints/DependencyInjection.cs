using Enjaz.Identity.Application;
using Enjaz.Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Identity.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityApplication();
        services.AddIdentityInfrastructure(configuration);
        services.AddIdentityEndpoints();

        return services;
    }

    public static IServiceCollection AddIdentityEndpoints(this IServiceCollection services)
    {
        return services;
    }
}
