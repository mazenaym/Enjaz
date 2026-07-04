using Enjaz.Jobs.Application;
using Enjaz.Jobs.Application.Jobs;
using Enjaz.Jobs.Endpoints.Realtime;
using Enjaz.Jobs.Infrastructure;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Jobs.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddJobsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJobsApplication();
        services.AddJobsInfrastructure(configuration);
        services.AddJobsEndpoints();
        return services;
    }

    public static IServiceCollection AddJobsEndpoints(this IServiceCollection services)
    {
        services.AddScoped<IJobsEventPublisher, SignalRJobsEventPublisher>();
        return services;
    }

    public static void AddJobsRecurringJobs()
    {
        RecurringJob.AddOrUpdate<IExpireTechnicianAssignmentJob>(
            "jobs-expire-technician-assignments",
            job => job.ExecuteAsync(CancellationToken.None),
            "*/5 * * * *");
    }
}
