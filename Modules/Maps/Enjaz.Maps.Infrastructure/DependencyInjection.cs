using Enjaz.Maps.Application.Maps;
using Enjaz.Maps.Infrastructure.Maps;
using Enjaz.Maps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Maps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMapsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<MapsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

        services.AddScoped<IMapsRepository, MapsRepository>();

        return services;
    }
}
