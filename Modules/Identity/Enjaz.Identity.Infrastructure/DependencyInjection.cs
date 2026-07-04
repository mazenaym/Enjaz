using Enjaz.Identity.Application.Auth;
using Enjaz.Identity.Infrastructure.Auth;
using Enjaz.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Enjaz.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IOtpHasher, OtpHasher>();
        services.AddScoped<IOtpRateLimiter, RedisOtpRateLimiter>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddScoped<IUserPasswordHasher, UserPasswordHasher>();
        services.AddScoped<IAppEnvironment, AppEnvironment>();
        services.AddScoped<FakeSmsSender>();
        services.AddScoped<UnavailableSmsSender>();
        services.AddScoped<ISmsSender>(serviceProvider =>
        {
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();

            return environment.IsDevelopment()
                ? serviceProvider.GetRequiredService<FakeSmsSender>()
                : serviceProvider.GetRequiredService<UnavailableSmsSender>();
        });

        return services;
    }
}
