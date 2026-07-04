using Enjaz.Notifications.Domain.Notifications;
using Enjaz.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Notifications.Application.Notifications;

public sealed class AdminNotificationsService(INotificationsRepository repository, INotificationService notificationService)
    : IAdminNotificationsService
{
    public async Task<Result<IReadOnlyCollection<NotificationResponse>>> GetNotificationsAsync(NotificationQuery query, CancellationToken cancellationToken = default)
    {
        var notifications = repository.QueryNotifications().AsNoTracking();
        if (query.UserId.HasValue) notifications = notifications.Where(notification => notification.UserId == query.UserId);
        if (!string.IsNullOrWhiteSpace(query.Type)) notifications = notifications.Where(notification => notification.Type == query.Type);
        if (!string.IsNullOrWhiteSpace(query.Channel)) notifications = notifications.Where(notification => notification.Channel == query.Channel);
        if (query.IsRead.HasValue) notifications = notifications.Where(notification => notification.IsRead == query.IsRead);
        if (query.FromDateUtc.HasValue) notifications = notifications.Where(notification => notification.CreatedAtUtc >= query.FromDateUtc);
        if (query.ToDateUtc.HasValue) notifications = notifications.Where(notification => notification.CreatedAtUtc <= query.ToDateUtc);
        return Result.Success<IReadOnlyCollection<NotificationResponse>>(await notifications.OrderByDescending(notification => notification.CreatedAtUtc).Take(500).Select(notification => UserNotificationsService.MapNotification(notification)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<NotificationDeliveryLogResponse>>> GetDeliveryLogsAsync(DeliveryLogQuery query, CancellationToken cancellationToken = default)
    {
        var logs = repository.QueryDeliveryLogs().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Status)) logs = logs.Where(log => log.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Channel)) logs = logs.Where(log => log.Channel == query.Channel);
        if (query.UserId.HasValue) logs = logs.Where(log => log.UserId == query.UserId);
        return Result.Success<IReadOnlyCollection<NotificationDeliveryLogResponse>>(await logs.OrderByDescending(log => log.CreatedAtUtc).Take(500).Select(log => MapLog(log)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<NotificationTemplateResponse>>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return Result.Success<IReadOnlyCollection<NotificationTemplateResponse>>(await repository.QueryTemplates().AsNoTracking().OrderBy(template => template.Type).ThenBy(template => template.Channel).Select(template => MapTemplate(template)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<NotificationTemplateResponse>> CreateTemplateAsync(NotificationTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = new NotificationTemplate
        {
            Type = request.Type.Trim(),
            Channel = NormalizeChannel(request.Channel),
            Language = string.IsNullOrWhiteSpace(request.Language) ? "ar" : request.Language.Trim(),
            TitleTemplate = request.TitleTemplate.Trim(),
            BodyTemplate = request.BodyTemplate.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        await repository.AddTemplateAsync(template, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapTemplate(template));
    }

    public async Task<Result<NotificationTemplateResponse>> UpdateTemplateAsync(Guid id, NotificationTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = await repository.GetTemplateAsync(id, cancellationToken);
        if (template is null) return Result.Failure<NotificationTemplateResponse>("template_not_found", "Template was not found.");
        template.Type = request.Type.Trim();
        template.Channel = NormalizeChannel(request.Channel);
        template.Language = string.IsNullOrWhiteSpace(request.Language) ? "ar" : request.Language.Trim();
        template.TitleTemplate = request.TitleTemplate.Trim();
        template.BodyTemplate = request.BodyTemplate.Trim();
        template.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapTemplate(template));
    }

    public async Task<Result> SetTemplateActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var template = await repository.GetTemplateAsync(id, cancellationToken);
        if (template is null) return Result.Failure("template_not_found", "Template was not found.");
        template.IsActive = isActive;
        template.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SendTestAsync(AdminTestNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var channels = request.Channels is { Count: > 0 } ? request.Channels : [NotificationChannels.InApp];
        return await notificationService.SendAsync(new SendNotificationRequest(request.UserId, request.Type, request.Title, request.Body, null, channels, request.SmsRecipient, request.EmailRecipient), cancellationToken);
    }

    private static NotificationTemplateResponse MapTemplate(NotificationTemplate template) => new(template.Id, template.Type, template.Channel, template.Language, template.TitleTemplate, template.BodyTemplate, template.IsActive, template.CreatedAtUtc, template.UpdatedAtUtc);
    private static NotificationDeliveryLogResponse MapLog(NotificationDeliveryLog log) => new(log.Id, log.NotificationId, log.UserId, log.Channel, log.Provider, log.Recipient, log.Status, log.AttemptCount, log.LastAttemptAtUtc, log.NextRetryAtUtc, log.ErrorMessage, log.CreatedAtUtc, log.UpdatedAtUtc);
    private static string NormalizeChannel(string channel) => channel.Equals("sms", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Sms : channel.Equals("email", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Email : channel.Equals("push", StringComparison.OrdinalIgnoreCase) ? NotificationChannels.Push : NotificationChannels.InApp;
}
