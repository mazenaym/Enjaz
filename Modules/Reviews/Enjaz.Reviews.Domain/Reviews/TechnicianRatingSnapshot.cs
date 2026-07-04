namespace Enjaz.Reviews.Domain.Reviews;

public sealed class TechnicianRatingSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TechnicianId { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime? LastReviewAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
