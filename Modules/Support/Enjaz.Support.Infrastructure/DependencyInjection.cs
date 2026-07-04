using Enjaz.Support.Application.Support;
using Enjaz.Support.Infrastructure.Persistence;
using Enjaz.Support.Infrastructure.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Support.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        services.AddDbContext<SupportDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ISupportRepository, SupportRepository>();
        return services;
    }
}
