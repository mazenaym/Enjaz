using Enjaz.Payments.Application.Payments;
using Enjaz.Payments.Domain.Payments;
using Enjaz.Payments.Infrastructure.Payments;
using Enjaz.Payments.Infrastructure.Persistence;
using Enjaz.Jobs.Application.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Payments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<PaymentsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        services.AddScoped<IJobPaymentSummaryLookupService, JobPaymentSummaryLookupService>();
        services.AddScoped<IPaymentProvider>(_ =>
        {
            var provider = configuration["Payments:Provider"] ?? PaymentProviders.Fake;
            return provider.Equals(PaymentProviders.Paymob, StringComparison.OrdinalIgnoreCase)
                ? new PaymobPaymentProvider(configuration)
                : new FakePaymentProvider();
        });

        return services;
    }
}
