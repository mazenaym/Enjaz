using Enjaz.Technicians.Application.Technicians;
using Enjaz.Maps.Application.Maps;
using Enjaz.Reviews.Application.Reviews;
using Enjaz.Technicians.Infrastructure.Persistence;
using Enjaz.Technicians.Infrastructure.Technicians;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Technicians.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTechniciansInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<TechniciansDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

        services.AddScoped<ITechniciansRepository, TechniciansRepository>();
        services.AddScoped<ITechnicianLookupService, TechnicianLookupService>();
        services.AddScoped<IReviewTechnicianLookupService, TechnicianLookupService>();

        return services;
    }
}
