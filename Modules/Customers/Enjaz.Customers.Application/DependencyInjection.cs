using Enjaz.Customers.Application.Customers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Customers.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomersApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}
