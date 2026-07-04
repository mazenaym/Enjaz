using Enjaz.Technicians.Application.Technicians;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Technicians.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTechniciansApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddScoped<ITechnicianService, TechnicianService>();

        return services;
    }
}
