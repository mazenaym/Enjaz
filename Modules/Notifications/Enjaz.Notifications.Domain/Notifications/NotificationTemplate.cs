namespace Enjaz.Notifications.Domain.Notifications;

public sealed class NotificationTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = NotificationTypes.General;
    public string Channel { get; set; } = NotificationChannels.InApp;
    public string Language { get; set; } = "ar";
    public string TitleTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
