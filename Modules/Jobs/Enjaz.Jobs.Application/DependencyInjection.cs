using Enjaz.Jobs.Application.Jobs;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Jobs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddJobsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ICustomerJobsService, JobsService>();
        services.AddScoped<IAdminJobsService, JobsService>();
        services.AddScoped<ITechnicianJobsService, JobsService>();
        services.AddScoped<IJobPaymentLookupService, JobsService>();
        services.AddScoped<IJobPaymentStatusService, JobsService>();
        services.AddScoped<IExpireTechnicianAssignmentJob, ExpireTechnicianAssignmentJob>();
        return services;
    }
}
