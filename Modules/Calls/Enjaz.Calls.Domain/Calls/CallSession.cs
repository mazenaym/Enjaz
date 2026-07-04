namespace Enjaz.Calls.Domain.Calls;

public sealed class CallSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Guid CustomerUserId { get; set; }
    public Guid TechnicianUserId { get; set; }
    public Guid TechnicianId { get; set; }
    public Guid InitiatedByUserId { get; set; }
    public string Provider { get; set; } = CallProviders.Fake;
    public string? ProviderCallId { get; set; }
    public string? MaskedNumber { get; set; }
    public string Status { get; set; } = CallStatuses.Created;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public int? DurationSeconds { get; set; }
    public string? RecordingUrl { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
