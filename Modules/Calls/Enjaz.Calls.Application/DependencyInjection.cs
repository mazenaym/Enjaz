using Enjaz.Calls.Application.Calls;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Calls.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCallsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICallsService, CallsService>();
        return services;
    }
}
