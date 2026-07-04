namespace Enjaz.Technicians.Domain.Technicians;

public sealed class TechnicianProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NationalId { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public string Status { get; set; } = TechnicianStatuses.Pending;
    public string AvailabilityStatus { get; set; } = TechnicianAvailabilityStatuses.Offline;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public DateTime? RejectedAtUtc { get; set; }
    public string? RejectionReason { get; set; }
    public ICollection<TechnicianDocument> Documents { get; set; } = new List<TechnicianDocument>();
    public ICollection<TechnicianSkill> Skills { get; set; } = new List<TechnicianSkill>();
    public ICollection<TechnicianAvailabilityHistory> AvailabilityHistory { get; set; } = new List<TechnicianAvailabilityHistory>();
}
