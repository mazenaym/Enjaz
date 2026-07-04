using Enjaz.Pricing.Application;
using Enjaz.Pricing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Pricing.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddPricingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPricingApplication();
        services.AddPricingInfrastructure(configuration);
        services.AddPricingEndpoints();

        return services;
    }

    public static IServiceCollection AddPricingEndpoints(this IServiceCollection services)
    {
        return services;
    }
}
