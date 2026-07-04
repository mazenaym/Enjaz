namespace Enjaz.Jobs.Domain.Jobs;

public sealed class JobAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Guid TechnicianId { get; set; }
    public Guid TechnicianUserId { get; set; }
    public string Status { get; set; } = JobAssignmentStatuses.Offered;
    public DateTime OfferedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public string? RejectionReason { get; set; }
}
