namespace Enjaz.Notifications.Domain.Notifications;

public sealed class NotificationDeliveryLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? NotificationId { get; set; }
    public Guid? UserId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Provider { get; set; } = NotificationProviders.Fake;
    public string? Recipient { get; set; }
    public string Status { get; set; } = NotificationDeliveryStatuses.Pending;
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public DateTime? NextRetryAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RequestJson { get; set; }
    public string? ResponseJson { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
