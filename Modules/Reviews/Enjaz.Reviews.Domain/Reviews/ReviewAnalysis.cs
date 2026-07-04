namespace Enjaz.Reviews.Domain.Reviews;

public sealed class ReviewAnalysis
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ReviewId { get; set; }
    public string Sentiment { get; set; } = ReviewSentiments.Neutral;
    public decimal Confidence { get; set; }
    public string? KeywordsJson { get; set; }
    public bool RequiresAdminAttention { get; set; }
    public string? RawAiResponseJson { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
