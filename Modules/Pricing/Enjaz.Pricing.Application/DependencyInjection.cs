using Enjaz.Pricing.Application.Pricing;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Pricing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPricingApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddScoped<PricingService>();
        services.AddScoped<IPricingCalculator>(provider => provider.GetRequiredService<PricingService>());
        services.AddScoped<IPricingAdminService>(provider => provider.GetRequiredService<PricingService>());

        return services;
    }
}
