using Enjaz.Customers.Application.Customers;
using Enjaz.Customers.Infrastructure.Customers;
using Enjaz.Customers.Infrastructure.Persistence;
using Enjaz.SharedKernel.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Customers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<CustomersDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

        services.AddScoped<ICustomersRepository, CustomersRepository>();
        services.AddScoped<ICustomerProfileProvisioner, CustomerProfileProvisioner>();

        return services;
    }
}
