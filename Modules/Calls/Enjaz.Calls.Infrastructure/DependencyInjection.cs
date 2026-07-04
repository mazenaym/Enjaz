using Enjaz.Calls.Application.Calls;
using Enjaz.Calls.Domain.Calls;
using Enjaz.Calls.Infrastructure.Calls;
using Enjaz.Calls.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Enjaz.Calls.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCallsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        services.Configure<CallsOptions>(options =>
        {
            var section = configuration.GetSection("Calls");
            options.Provider = section["Provider"] ?? CallProviders.Fake;
            options.Callera.BaseUrl = section["Callera:BaseUrl"];
            options.Callera.ApiKey = section["Callera:ApiKey"];
            options.Callera.WebhookSecret = section["Callera:WebhookSecret"];
        });
        services.AddDbContext<CallsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ICallsRepository, CallsRepository>();
        services.AddScoped<FakeCallProvider>();
        services.AddScoped<CalleraCallProvider>();
        services.AddScoped<ICallProvider>(provider =>
        {
            var selected = provider.GetRequiredService<IOptions<CallsOptions>>().Value.Provider;
            return string.Equals(selected, CallProviders.Callera, StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<CalleraCallProvider>()
                : provider.GetRequiredService<FakeCallProvider>();
        });
        return services;
    }
}
