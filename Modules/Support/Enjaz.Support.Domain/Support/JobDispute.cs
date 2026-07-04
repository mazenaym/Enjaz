namespace Enjaz.Support.Domain.Support;

public sealed class JobDispute
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Guid OpenedByUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = JobDisputeStatuses.Open;
    public string? Resolution { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
}
