namespace Enjaz.Jobs.Domain.Jobs;

public sealed class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string JobNumber { get; set; } = string.Empty;
    public Guid CustomerUserId { get; set; }
    public Guid? CustomerProfileId { get; set; }
    public Guid CustomerAddressId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid? ServiceTierId { get; set; }
    public Guid? AiClassificationId { get; set; }
    public Guid PriceSnapshotId { get; set; }
    public Guid? ServiceZoneId { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public Guid? AssignedTechnicianUserId { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = JobStatuses.WaitingForPayment;
    public DateTime? ScheduledAtUtc { get; set; }
    public DateTime? PreferredTimeWindowStartUtc { get; set; }
    public DateTime? PreferredTimeWindowEndUtc { get; set; }
    public string Currency { get; set; } = "EGP";
    public decimal EstimatedTotalAmount { get; set; }
    public decimal EstimatedDepositAmount { get; set; }
    public bool RequiresInspection { get; set; }
    public string? CancellationReason { get; set; }
    public Guid? CancelledByUserId { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
