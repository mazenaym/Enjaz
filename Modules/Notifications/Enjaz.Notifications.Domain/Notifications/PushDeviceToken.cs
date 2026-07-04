namespace Enjaz.Notifications.Domain.Notifications;

public sealed class PushDeviceToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? Platform { get; set; }
    public string Provider { get; set; } = NotificationProviders.Expo;
    public string? DeviceId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? LastSeenAtUtc { get; set; }
}
