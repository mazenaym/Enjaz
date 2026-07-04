namespace Enjaz.Notifications.Application.Notifications;

public sealed record NotificationResponse(Guid Id, Guid UserId, string Channel, string Type, string Title, string Body, string? DataJson, bool IsRead, DateTime? ReadAtUtc, DateTime CreatedAtUtc);
public sealed record NotificationPreferenceResponse(Guid Id, Guid UserId, string Channel, bool IsEnabled, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
public sealed record UpdateNotificationPreferenceRequest(IReadOnlyCollection<NotificationPreferenceItem> Preferences);
public sealed record NotificationPreferenceItem(string Channel, bool IsEnabled);
public sealed record RegisterDeviceTokenRequest(string Token, string? Platform, string? Provider, string? DeviceId);
public sealed record NotificationTemplateRequest(string Type, string Channel, string? Language, string TitleTemplate, string BodyTemplate);
public sealed record NotificationTemplateResponse(Guid Id, string Type, string Channel, string Language, string TitleTemplate, string BodyTemplate, bool IsActive, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
public sealed record NotificationDeliveryLogResponse(Guid Id, Guid? NotificationId, Guid? UserId, string Channel, string Provider, string? Recipient, string Status, int AttemptCount, DateTime? LastAttemptAtUtc, DateTime? NextRetryAtUtc, string? ErrorMessage, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
public sealed record AdminTestNotificationRequest(Guid UserId, string Type, string Title, string Body, IReadOnlyCollection<string>? Channels, string? SmsRecipient, string? EmailRecipient);
public sealed record NotificationQuery(Guid? UserId, string? Type, string? Channel, bool? IsRead, DateTime? FromDateUtc, DateTime? ToDateUtc);
public sealed record DeliveryLogQuery(string? Status, string? Channel, Guid? UserId);
public sealed record SendNotificationRequest(Guid UserId, string Type, string Title, string Body, IReadOnlyDictionary<string, string?>? Data, IReadOnlyCollection<string> Channels, string? SmsRecipient = null, string? EmailRecipient = null);
public sealed record ProviderNotificationRequest(Guid? UserId, string Recipient, string Type, string Title, string Body, string? DataJson);
public sealed record ProviderNotificationResult(bool IsSuccess, string Provider, string? ResponseJson, string? ErrorMessage);
public sealed record RenderedNotificationTemplate(string Title, string Body);
