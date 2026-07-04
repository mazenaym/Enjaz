namespace Enjaz.Technicians.Domain.Technicians;

public sealed class TechnicianAvailabilityHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TechnicianId { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = TechnicianAvailabilityStatuses.Offline;
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? ChangedByUserId { get; set; }
    public TechnicianProfile? Technician { get; set; }
}
