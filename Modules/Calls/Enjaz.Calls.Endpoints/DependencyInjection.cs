using Enjaz.Calls.Application;
using Enjaz.Calls.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Calls.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddCallsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCallsApplication();
        services.AddCallsInfrastructure(configuration);
        return services;
    }
}
