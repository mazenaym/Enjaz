using Enjaz.Wallets.Application;
using Enjaz.Wallets.Application.Wallets;
using Enjaz.Wallets.Infrastructure;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Wallets.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddWalletsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWalletsApplication();
        services.AddWalletsInfrastructure(configuration);
        return services;
    }

    public static void AddWalletsRecurringJobs()
    {
        RecurringJob.AddOrUpdate<IReleaseTechnicianEarningsJob>(
            "wallets-release-technician-earnings",
            job => job.ExecuteAsync(CancellationToken.None),
            "*/10 * * * *");
    }
}
