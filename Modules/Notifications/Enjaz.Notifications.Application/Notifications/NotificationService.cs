using System.Text.Json;
using Enjaz.Notifications.Domain.Notifications;
using Enjaz.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Notifications.Application.Notifications;

public sealed class NotificationService(
    INotificationsRepository repository,
    INotificationTemplateRenderer templateRenderer,
    INotificationDispatcher dispatcher)
    : INotificationService
{
    public Task<Result> SendInAppAsync(Guid userId, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default)
        => SendAsync(new SendNotificationRequest(userId, type, title, body, data, [NotificationChannels.InApp]), cancellationToken);

    public Task<Result> SendSmsAsync(Guid userId, string phoneNumber, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default)
        => SendAsync(new SendNotificationRequest(userId, type, title, body, data, [NotificationChannels.Sms], SmsRecipient: phoneNumber), cancellationToken);

    public Task<Result> SendPushAsync(Guid userId, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default)
        => SendAsync(new SendNotificationRequest(userId, type, title, body, data, [NotificationChannels.Push]), cancellationToken);

    public Task<Result> SendEmailAsync(Guid userId, string email, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default)
        => SendAsync(new SendNotificationRequest(userId, type, title, body, data, [NotificationChannels.Email], EmailRecipient: email), cancellationToken);

    public async Task<Result> SendToUserAsync(Guid userId, string type, IReadOnlyDictionary<string, string?> templateData, IReadOnlyCollection<string> channels, CancellationToken cancellationToken = default)
    {
        return await SendAsync(new SendNotificationRequest(userId, type, type, type, templateData, channels), cancellationToken);
    }

    public async Task<Result> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var dataJson = request.Data is null ? null : JsonSerializer.Serialize(request.Data);
        foreach (var rawChannel in request.Channels.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var channel = NormalizeChannel(rawChannel);
            if (!await IsEnabledAsync(request.UserId, channel, cancellationToken)) continue;

            var rendered = await templateRenderer.RenderAsync(request.Type, channel, request.Data ?? new Dictionary<string, string?>(), fallbackTitle: request.Title, fallbackBody: request.Body, cancellationToken: cancellationToken);
            Notification? notification = null;
            if (channel is NotificationChannels.InApp or NotificationChannels.Push)
            {
                notification = new Notification
                {
                    UserId = request.UserId,
                    Channel = channel,
                    Type = request.Type,
                    Title = rendered.Title,
                    Body = rendered.Body,
                    DataJson = dataJson,
                    CreatedAtUtc = DateTime.UtcNow
                };
                await repository.AddNotificationAsync(notification, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);
            }

            if (channel == NotificationChannels.Sms && !string.IsNullOrWhiteSpace(request.SmsRecipient))
            {
                await CreateAndDispatchLogAsync(notification?.Id, request.UserId, channel, request.SmsRecipient, request.Type, rendered.Title, rendered.Body, dataJson, cancellationToken);
            }
            else if (channel == NotificationChannels.Email && !string.IsNullOrWhiteSpace(request.EmailRecipient))
            {
                await CreateAndDispatchLogAsync(notification?.Id, request.UserId, channel, request.EmailRecipient, request.Type, rendered.Title, rendered.Body, dataJson, cancellationToken);
            }
            else if (channel == NotificationChannels.Push)
            {
                var tokens = await repository.QueryDeviceTokens().Where(token => token.UserId == request.UserId && token.IsActive).ToArrayAsync(cancellationToken);
                foreach (var token in tokens)
                {
                    await CreateAndDispatchLogAsync(notification?.Id, request.UserId, channel, token.Token, request.Type, rendered.Title, rendered.Body, dataJson, cancellationToken);
                }
            }
        }

        return Result.Success();
    }

    private async Task CreateAndDispatchLogAsync(Guid? notificationId, Guid userId, string channel, string recipient, string type, string title, string body, string? dataJson, CancellationToken cancellationToken)
    {
        var log = new NotificationDeliveryLog
        {
            NotificationId = notificationId,
            UserId = userId,
            Channel = channel,
            Provider = NotificationProviders.Fake,
            Recipient = recipient,
            Status = NotificationDeliveryStatuses.Pending,
            RequestJson = JsonSerializer.Serialize(new ProviderNotificationRequest(userId, recipient, type, title, body, dataJson)),
            CreatedAtUtc = DateTime.UtcNow
        };
        await repository.AddDeliveryLogAsync(log, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await dispatcher.DispatchAsync(log, cancellationToken);
    }

    private async Task<bool> IsEnabledAsync(Guid userId, string channel, CancellationToken cancellationToken)
    {
        var preference = await repository.GetPreferenceAsync(userId, channel, cancellationToken);
        return preference?.IsEnabled ?? true;
    }

    private static string NormalizeChannel(string channel) => channel.Equals("sms", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Sms : channel.Equals("email", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Email : channel.Equals("push", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Push : NotificationChannels.InApp;
}
