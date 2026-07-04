using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Enjaz.Notifications.Infrastructure.Notifications;
using Enjaz.Notifications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<NotificationsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<ISmsNotificationProvider>(provider => CreateSmsProvider(provider, configuration));
        services.AddScoped<IPushNotificationProvider>(provider => CreatePushProvider(provider, configuration));
        services.AddScoped<IEmailNotificationProvider>(provider => CreateEmailProvider(provider, configuration));
        return services;
    }

    private static ISmsNotificationProvider CreateSmsProvider(IServiceProvider provider, IConfiguration configuration)
    {
        var selected = configuration["Notifications:SmsProvider"] ?? NotificationProviders.Fake;
        if (selected.Equals(NotificationProviders.VictoryLink, StringComparison.OrdinalIgnoreCase)) return ActivatorUtilities.CreateInstance<VictoryLinkSmsProvider>(provider, configuration);
        if (selected.Equals(NotificationProviders.Cequens, StringComparison.OrdinalIgnoreCase)) return ActivatorUtilities.CreateInstance<CequensSmsProvider>(provider, configuration);
        return ActivatorUtilities.CreateInstance<FakeSmsNotificationProvider>(provider, configuration);
    }

    private static IPushNotificationProvider CreatePushProvider(IServiceProvider provider, IConfiguration configuration)
    {
        var selected = configuration["Notifications:PushProvider"] ?? NotificationProviders.Fake;
        if (selected.Equals(NotificationProviders.Firebase, StringComparison.OrdinalIgnoreCase)) return ActivatorUtilities.CreateInstance<FirebasePushProvider>(provider, configuration);
        if (selected.Equals(NotificationProviders.Expo, StringComparison.OrdinalIgnoreCase)) return ActivatorUtilities.CreateInstance<ExpoPushProvider>(provider, configuration);
        return ActivatorUtilities.CreateInstance<FakePushNotificationProvider>(provider, configuration);
    }

    private static IEmailNotificationProvider CreateEmailProvider(IServiceProvider provider, IConfiguration configuration)
    {
        var selected = configuration["Notifications:EmailProvider"] ?? NotificationProviders.Fake;
        if (selected.Equals(NotificationProviders.Email, StringComparison.OrdinalIgnoreCase) || selected.Equals("Smtp", StringComparison.OrdinalIgnoreCase)) return ActivatorUtilities.CreateInstance<SmtpEmailProvider>(provider, configuration);
        return ActivatorUtilities.CreateInstance<FakeEmailNotificationProvider>(provider, configuration);
    }
}
