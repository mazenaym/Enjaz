using Enjaz.Jobs.Application.Jobs;
using Enjaz.Jobs.Infrastructure.Jobs;
using Enjaz.Jobs.Infrastructure.Persistence;
using Enjaz.Wallets.Application.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Jobs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddJobsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<JobsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IJobsRepository, JobsRepository>();
        services.AddScoped<ICustomerLookupService, CustomerLookupService>();
        services.AddScoped<IPricingSnapshotLookupService, PricingSnapshotLookupService>();
        services.AddScoped<IServiceZoneLookupService, ServiceZoneLookupService>();
        services.AddScoped<IJobWalletLookupService, JobWalletLookupService>();

        return services;
    }
}
