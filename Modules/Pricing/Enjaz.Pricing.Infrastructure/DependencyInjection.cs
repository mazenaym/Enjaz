using Enjaz.Pricing.Application.Pricing;
using Enjaz.Pricing.Infrastructure.Persistence;
using Enjaz.Pricing.Infrastructure.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Pricing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPricingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<PricingDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPricingRepository, PricingRepository>();

        return services;
    }
}
