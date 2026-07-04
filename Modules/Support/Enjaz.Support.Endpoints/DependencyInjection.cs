using Enjaz.Support.Application;
using Enjaz.Support.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Support.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSupportApplication();
        services.AddSupportInfrastructure(configuration);
        return services;
    }
}
