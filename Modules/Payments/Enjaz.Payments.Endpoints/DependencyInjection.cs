using Enjaz.Payments.Application;
using Enjaz.Payments.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Payments.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPaymentsApplication();
        services.AddPaymentsInfrastructure(configuration);
        return services;
    }
}
