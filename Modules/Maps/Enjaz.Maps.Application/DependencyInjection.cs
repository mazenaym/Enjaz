using Enjaz.Maps.Application.Maps;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Maps.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMapsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddScoped<IMapsService, MapsService>();

        return services;
    }
}
