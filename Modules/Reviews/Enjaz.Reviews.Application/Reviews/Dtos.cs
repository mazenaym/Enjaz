namespace Enjaz.Reviews.Application.Reviews;

public sealed record CreateReviewRequest(int Rating, string? Comment);
public sealed record ReviewResponse(Guid Id, Guid JobId, Guid CustomerUserId, Guid TechnicianId, Guid TechnicianUserId, int Rating, string? Comment, bool IsVisible, DateTime CreatedAtUtc, ReviewAnalysisResponse? Analysis);
public sealed record ReviewAnalysisResponse(string Sentiment, decimal Confidence, bool RequiresAdminAttention, string? KeywordsJson);
public sealed record TechnicianRatingSummaryResponse(Guid TechnicianId, decimal AverageRating, int TotalReviews, DateTime? LastReviewAtUtc);
public sealed record ReviewQuery(Guid? TechnicianId, Guid? JobId, bool? IsVisible);
public sealed record ReviewJobLookupResult(Guid JobId, Guid CustomerUserId, Guid? AssignedTechnicianId, Guid? AssignedTechnicianUserId, string Status);
public sealed record ReviewSentimentResult(string Sentiment, decimal Confidence, IReadOnlyCollection<string> Keywords, bool RequiresAdminAttention, string RawResponseJson);
