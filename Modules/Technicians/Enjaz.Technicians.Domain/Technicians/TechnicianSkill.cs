namespace Enjaz.Technicians.Domain.Technicians;

public sealed class TechnicianSkill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TechnicianId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public string? SkillLevel { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public TechnicianProfile? Technician { get; set; }
}
