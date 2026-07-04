namespace Enjaz.Identity.Domain.Otp;

public sealed class OtpCode
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string PhoneNumber { get; set; } = string.Empty;

    public string CodeHash { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? UsedAtUtc { get; set; }

    public int AttemptCount { get; set; }

    public int MaxAttempts { get; set; } = 5;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
