using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Notifications.Application.Notifications;

public sealed class RetryFailedNotificationsJob(INotificationsRepository repository, INotificationDispatcher dispatcher) : IRetryFailedNotificationsJob
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var logs = await repository.QueryDeliveryLogs()
            .Where(log => (log.Status == NotificationDeliveryStatuses.Failed || log.Status == NotificationDeliveryStatuses.Retrying) && log.NextRetryAtUtc.HasValue && log.NextRetryAtUtc <= now && log.AttemptCount < 5)
            .OrderBy(log => log.NextRetryAtUtc)
            .Take(100)
            .ToArrayAsync(cancellationToken);

        foreach (var log in logs)
        {
            log.Status = NotificationDeliveryStatuses.Retrying;
            log.UpdatedAtUtc = now;
            await repository.SaveChangesAsync(cancellationToken);
            await dispatcher.DispatchAsync(log, cancellationToken);
        }
    }
}
