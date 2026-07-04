namespace Enjaz.Reviews.Domain.Reviews;

public sealed class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Guid CustomerUserId { get; set; }
    public Guid TechnicianId { get; set; }
    public Guid TechnicianUserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
