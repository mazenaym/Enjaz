using System.Text.Json;
using Enjaz.Reviews.Domain.Reviews;

namespace Enjaz.Reviews.Application.Reviews;

public sealed class FakeReviewSentimentAnalyzer : IReviewSentimentAnalyzer
{
    public Task<ReviewSentimentResult> AnalyzeAsync(int rating, string? comment, CancellationToken cancellationToken = default)
    {
        var normalized = comment?.ToLowerInvariant() ?? string.Empty;
        var negative = rating <= 2 || normalized.Contains("bad", StringComparison.OrdinalIgnoreCase) || normalized.Contains("late", StringComparison.OrdinalIgnoreCase) || normalized.Contains("poor", StringComparison.OrdinalIgnoreCase);
        var sentiment = negative ? ReviewSentiments.Negative : rating >= 4 ? ReviewSentiments.Positive : ReviewSentiments.Neutral;
        var keywords = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Take(8).ToArray();
        var result = new ReviewSentimentResult(sentiment, negative ? 0.91m : 0.78m, keywords, negative, JsonSerializer.Serialize(new { provider = "Fake", sentiment }));
        return Task.FromResult(result);
    }
}
