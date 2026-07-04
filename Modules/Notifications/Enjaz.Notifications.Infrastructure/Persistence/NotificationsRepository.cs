using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Notifications.Infrastructure.Persistence;

public sealed class NotificationsRepository(NotificationsDbContext dbContext) : INotificationsRepository
{
    public IQueryable<Notification> QueryNotifications() => dbContext.Notifications;
    public IQueryable<NotificationTemplate> QueryTemplates() => dbContext.NotificationTemplates;
    public IQueryable<NotificationDeliveryLog> QueryDeliveryLogs() => dbContext.NotificationDeliveryLogs;
    public IQueryable<UserNotificationPreference> QueryPreferences() => dbContext.UserNotificationPreferences;
    public IQueryable<PushDeviceToken> QueryDeviceTokens() => dbContext.PushDeviceTokens;
    public Task<Notification?> GetNotificationAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.Notifications.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    public Task<NotificationTemplate?> GetTemplateAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.NotificationTemplates.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    public Task<NotificationDeliveryLog?> GetDeliveryLogAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.NotificationDeliveryLogs.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    public Task<UserNotificationPreference?> GetPreferenceAsync(Guid userId, string channel, CancellationToken cancellationToken = default) => dbContext.UserNotificationPreferences.FirstOrDefaultAsync(item => item.UserId == userId && item.Channel == channel, cancellationToken);
    public Task<PushDeviceToken?> GetDeviceTokenAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.PushDeviceTokens.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    public Task<PushDeviceToken?> GetDeviceTokenByTokenAsync(string token, CancellationToken cancellationToken = default) => dbContext.PushDeviceTokens.FirstOrDefaultAsync(item => item.Token == token, cancellationToken);
    public async Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default) => await dbContext.Notifications.AddAsync(notification, cancellationToken);
    public async Task AddTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken = default) => await dbContext.NotificationTemplates.AddAsync(template, cancellationToken);
    public async Task AddDeliveryLogAsync(NotificationDeliveryLog log, CancellationToken cancellationToken = default) => await dbContext.NotificationDeliveryLogs.AddAsync(log, cancellationToken);
    public async Task AddPreferenceAsync(UserNotificationPreference preference, CancellationToken cancellationToken = default) => await dbContext.UserNotificationPreferences.AddAsync(preference, cancellationToken);
    public async Task AddDeviceTokenAsync(PushDeviceToken token, CancellationToken cancellationToken = default) => await dbContext.PushDeviceTokens.AddAsync(token, cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
