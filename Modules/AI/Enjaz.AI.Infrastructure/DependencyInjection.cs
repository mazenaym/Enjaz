using Enjaz.AI.Application.AI;
using Enjaz.AI.Infrastructure.AI;
using Enjaz.AI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.AI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<AiDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAiRepository, AiRepository>();
        services.AddScoped<IAiProvider>(_ =>
        {
            var provider = configuration["Ai:Provider"] ?? "Fake";
            return string.Equals(provider, "Gemini", StringComparison.OrdinalIgnoreCase)
                ? new GeminiIssueClassifierProvider(configuration)
                : new FakeIssueClassifierProvider();
        });

        return services;
    }
}
