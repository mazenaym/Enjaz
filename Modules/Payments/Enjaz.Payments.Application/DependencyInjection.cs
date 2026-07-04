using Enjaz.Payments.Application.Payments;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Payments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IPaymentsService, PaymentsService>();
        services.AddScoped<IAdminPaymentsService, PaymentsService>();
        return services;
    }
}
