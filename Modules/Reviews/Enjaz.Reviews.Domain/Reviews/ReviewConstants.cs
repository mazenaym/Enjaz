namespace Enjaz.Reviews.Domain.Reviews;

public static class ReviewSentiments
{
    public const string Positive = "Positive";
    public const string Neutral = "Neutral";
    public const string Negative = "Negative";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { Positive, Neutral, Negative };
}
