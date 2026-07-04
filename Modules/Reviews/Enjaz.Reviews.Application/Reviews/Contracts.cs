using Enjaz.Reviews.Domain.Reviews;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Reviews.Application.Reviews;

public interface IReviewsService
{
    Task<Result<ReviewResponse>> CreateForJobAsync(Guid jobId, CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<Result<ReviewResponse>> GetForJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ReviewResponse>>> GetMyTechnicianReviewsAsync(CancellationToken cancellationToken = default);
    Task<Result<TechnicianRatingSummaryResponse>> GetMyTechnicianRatingSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ReviewResponse>>> GetTechnicianReviewsAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ReviewResponse>>> GetReviewsAsync(ReviewQuery query, CancellationToken cancellationToken = default);
    Task<Result<ReviewResponse>> SetVisibilityAsync(Guid id, bool isVisible, CancellationToken cancellationToken = default);
}

public interface IReviewsRepository
{
    IQueryable<Review> QueryReviews();
    IQueryable<ReviewAnalysis> QueryAnalysis();
    IQueryable<TechnicianRatingSnapshot> QueryRatingSnapshots();
    Task<Review?> GetReviewAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Review?> GetReviewByJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<ReviewAnalysis?> GetAnalysisByReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<TechnicianRatingSnapshot?> GetRatingSnapshotAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task AddReviewAsync(Review review, CancellationToken cancellationToken = default);
    Task AddAnalysisAsync(ReviewAnalysis analysis, CancellationToken cancellationToken = default);
    Task AddRatingSnapshotAsync(TechnicianRatingSnapshot snapshot, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IReviewJobLookupService
{
    Task<ReviewJobLookupResult?> GetReviewableJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}

public interface IReviewTechnicianLookupService
{
    Task<Guid?> GetTechnicianIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateRatingAsync(Guid technicianId, decimal averageRating, int totalReviews, CancellationToken cancellationToken = default);
}

public interface IReviewSentimentAnalyzer
{
    Task<ReviewSentimentResult> AnalyzeAsync(int rating, string? comment, CancellationToken cancellationToken = default);
}
