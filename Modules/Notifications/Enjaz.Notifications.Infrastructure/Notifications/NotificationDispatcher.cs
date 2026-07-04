using System.Text.Json;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;

namespace Enjaz.Notifications.Infrastructure.Notifications;

public sealed class NotificationDispatcher(
    INotificationsRepository repository,
    ISmsNotificationProvider smsProvider,
    IPushNotificationProvider pushProvider,
    IEmailNotificationProvider emailProvider)
    : INotificationDispatcher
{
    public async Task DispatchAsync(NotificationDeliveryLog log, CancellationToken cancellationToken = default)
    {
        var request = string.IsNullOrWhiteSpace(log.RequestJson)
            ? new ProviderNotificationRequest(log.UserId, log.Recipient ?? string.Empty, log.Channel, log.Channel, log.Channel, null)
            : JsonSerializer.Deserialize<ProviderNotificationRequest>(log.RequestJson) ?? new ProviderNotificationRequest(log.UserId, log.Recipient ?? string.Empty, log.Channel, log.Channel, log.Channel, null);

        var now = DateTime.UtcNow;
        log.AttemptCount += 1;
        log.LastAttemptAtUtc = now;
        var result = log.Channel switch
        {
            NotificationChannels.Sms => await smsProvider.SendAsync(request, cancellationToken),
            NotificationChannels.Push => await pushProvider.SendAsync(request, cancellationToken),
            NotificationChannels.Email => await emailProvider.SendAsync(request, cancellationToken),
            _ => new ProviderNotificationResult(true, NotificationProviders.Fake, "{\"inApp\":true}", null)
        };

        log.Provider = result.Provider;
        log.ResponseJson = result.ResponseJson;
        log.ErrorMessage = result.ErrorMessage;
        log.Status = result.IsSuccess ? NotificationDeliveryStatuses.Sent : NotificationDeliveryStatuses.Failed;
        log.NextRetryAtUtc = result.IsSuccess || log.AttemptCount >= 5 ? null : now.AddMinutes(Math.Min(60, Math.Pow(2, log.AttemptCount)));
        log.UpdatedAtUtc = now;
        await repository.SaveChangesAsync(cancellationToken);
    }
}
