using Enjaz.Notifications.Application.Notifications;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Notifications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<INotificationTemplateRenderer, NotificationTemplateRenderer>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserNotificationsService, UserNotificationsService>();
        services.AddScoped<IAdminNotificationsService, AdminNotificationsService>();
        services.AddScoped<IRetryFailedNotificationsJob, RetryFailedNotificationsJob>();
        return services;
    }
}
