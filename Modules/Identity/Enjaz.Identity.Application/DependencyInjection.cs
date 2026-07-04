using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        return services;
    }
}
