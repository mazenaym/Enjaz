using FluentValidation;
using Enjaz.Identity.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
