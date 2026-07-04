using Enjaz.Support.Application.Support;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Support.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ISupportService, SupportService>();
        return services;
    }
}
