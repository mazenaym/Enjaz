using Enjaz.Wallets.Application.Wallets;
using Enjaz.Wallets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Wallets.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWalletsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<WalletsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IWalletsRepository, WalletsRepository>();
        return services;
    }
}
