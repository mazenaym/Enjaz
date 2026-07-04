using Enjaz.Wallets.Application.Wallets;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Wallets.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWalletsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ILedgerService, LedgerService>();
        services.AddScoped<IPaymentLedgerService, PaymentLedgerService>();
        services.AddScoped<IRefundLedgerService, RefundLedgerService>();
        services.AddScoped<IWalletsService, WalletsService>();
        services.AddScoped<IAdminWalletsService, WalletsService>();
        services.AddScoped<IReleaseTechnicianEarningsJob, ReleaseTechnicianEarningsJob>();
        return services;
    }
}
