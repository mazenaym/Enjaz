namespace Enjaz.Jobs.Domain.Jobs;

public sealed class JobStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
    public Guid? ChangedByUserId { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
