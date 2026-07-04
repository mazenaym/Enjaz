using Enjaz.Notifications.Domain.Notifications;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Notifications.Application.Notifications;

public sealed class UserNotificationsService(INotificationsRepository repository, ICurrentUserContext currentUserContext)
    : IUserNotificationsService
{
    public async Task<Result<IReadOnlyCollection<NotificationResponse>>> GetMyNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var notifications = await repository.QueryNotifications()
            .AsNoTracking()
            .Where(notification => notification.UserId == currentUserContext.UserId)
            .OrderByDescending(notification => notification.CreatedAtUtc)
            .Take(100)
            .Select(notification => MapNotification(notification))
            .ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<NotificationResponse>>(notifications);
    }

    public async Task<Result> MarkReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetNotificationAsync(id, cancellationToken);
        if (notification is null || notification.UserId != currentUserContext.UserId) return Result.Failure("notification_not_found", "Notification was not found.");
        notification.IsRead = true;
        notification.ReadAtUtc ??= DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        var notifications = await repository.QueryNotifications().Where(notification => notification.UserId == currentUserContext.UserId && !notification.IsRead).ToArrayAsync(cancellationToken);
        var now = DateTime.UtcNow;
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAtUtc = now;
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<NotificationPreferenceResponse>>> GetMyPreferencesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDefaultPreferencesAsync(currentUserContext.UserId, cancellationToken);
        var preferences = await repository.QueryPreferences().AsNoTracking().Where(preference => preference.UserId == currentUserContext.UserId).OrderBy(preference => preference.Channel).Select(preference => MapPreference(preference)).ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<NotificationPreferenceResponse>>(preferences);
    }

    public async Task<Result<IReadOnlyCollection<NotificationPreferenceResponse>>> UpdateMyPreferencesAsync(UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var item in request.Preferences)
        {
            var channel = NormalizeChannel(item.Channel);
            var preference = await repository.GetPreferenceAsync(currentUserContext.UserId, channel, cancellationToken);
            if (preference is null)
            {
                preference = new UserNotificationPreference { UserId = currentUserContext.UserId, Channel = channel, IsEnabled = item.IsEnabled, CreatedAtUtc = now };
                await repository.AddPreferenceAsync(preference, cancellationToken);
            }
            else
            {
                preference.IsEnabled = item.IsEnabled;
                preference.UpdatedAtUtc = now;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
        return await GetMyPreferencesAsync(cancellationToken);
    }

    public async Task<Result> RegisterDeviceTokenAsync(RegisterDeviceTokenRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var token = await repository.GetDeviceTokenByTokenAsync(request.Token, cancellationToken);
        if (token is null)
        {
            token = new PushDeviceToken
            {
                UserId = currentUserContext.UserId,
                Token = request.Token.Trim(),
                Platform = request.Platform?.Trim(),
                Provider = string.IsNullOrWhiteSpace(request.Provider) ? NotificationProviders.Expo : request.Provider.Trim(),
                DeviceId = request.DeviceId?.Trim(),
                IsActive = true,
                CreatedAtUtc = now,
                LastSeenAtUtc = now
            };
            await repository.AddDeviceTokenAsync(token, cancellationToken);
        }
        else
        {
            token.UserId = currentUserContext.UserId;
            token.Platform = request.Platform?.Trim();
            token.Provider = string.IsNullOrWhiteSpace(request.Provider) ? token.Provider : request.Provider.Trim();
            token.DeviceId = request.DeviceId?.Trim();
            token.IsActive = true;
            token.UpdatedAtUtc = now;
            token.LastSeenAtUtc = now;
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteDeviceTokenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var token = await repository.GetDeviceTokenAsync(id, cancellationToken);
        if (token is null || token.UserId != currentUserContext.UserId) return Result.Failure("device_token_not_found", "Device token was not found.");
        token.IsActive = false;
        token.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task EnsureDefaultPreferencesAsync(Guid userId, CancellationToken cancellationToken)
    {
        foreach (var channel in new[] { NotificationChannels.InApp, NotificationChannels.Sms, NotificationChannels.Push, NotificationChannels.Email })
        {
            if (await repository.GetPreferenceAsync(userId, channel, cancellationToken) is null)
            {
                await repository.AddPreferenceAsync(new UserNotificationPreference { UserId = userId, Channel = channel, IsEnabled = true, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
    }

    internal static NotificationResponse MapNotification(Notification notification) => new(notification.Id, notification.UserId, notification.Channel, notification.Type, notification.Title, notification.Body, notification.DataJson, notification.IsRead, notification.ReadAtUtc, notification.CreatedAtUtc);
    internal static NotificationPreferenceResponse MapPreference(UserNotificationPreference preference) => new(preference.Id, preference.UserId, preference.Channel, preference.IsEnabled, preference.CreatedAtUtc, preference.UpdatedAtUtc);
    private static string NormalizeChannel(string channel) => channel.Equals("sms", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Sms : channel.Equals("email", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Email : channel.Equals("push", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Push : NotificationChannels.InApp;
}
