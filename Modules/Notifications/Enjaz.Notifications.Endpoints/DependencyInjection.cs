using Enjaz.Notifications.Application;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Infrastructure;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Notifications.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNotificationsApplication();
        services.AddNotificationsInfrastructure(configuration);
        return services;
    }

    public static void AddNotificationsRecurringJobs()
    {
        RecurringJob.AddOrUpdate<IRetryFailedNotificationsJob>(
            "notifications-retry-failed",
            job => job.ExecuteAsync(CancellationToken.None),
            "*/5 * * * *");
    }
}
