using System.Text.Json;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Reviews.Domain.Reviews;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Reviews.Application.Reviews;

public sealed class ReviewsService(
    IReviewsRepository repository,
    IReviewJobLookupService jobLookupService,
    IReviewTechnicianLookupService technicianLookupService,
    IReviewSentimentAnalyzer sentimentAnalyzer,
    ICurrentUserContext currentUserContext,
    INotificationService notificationService) : IReviewsService
{
    public async Task<Result<ReviewResponse>> CreateForJobAsync(Guid jobId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId == Guid.Empty) return Result.Failure<ReviewResponse>("unauthorized", "Authentication is required.");
        var job = await jobLookupService.GetReviewableJobAsync(jobId, cancellationToken);
        if (job is null) return Result.Failure<ReviewResponse>("job_not_found", "Job was not found.");
        if (!string.Equals(job.Status, "Completed", StringComparison.OrdinalIgnoreCase)) return Result.Failure<ReviewResponse>("job_not_completed", "Only completed jobs can be reviewed.");
        if (job.CustomerUserId != currentUserContext.UserId) return Result.Failure<ReviewResponse>("job_not_owned", "Only the customer who owns the job can review it.");
        if (!job.AssignedTechnicianId.HasValue || !job.AssignedTechnicianUserId.HasValue) return Result.Failure<ReviewResponse>("job_technician_missing", "Job does not have an assigned technician.");
        if (await repository.GetReviewByJobAsync(jobId, cancellationToken) is not null) return Result.Failure<ReviewResponse>("review_exists", "A review already exists for this job.");

        var now = DateTime.UtcNow;
        var review = new Review { JobId = job.JobId, CustomerUserId = job.CustomerUserId, TechnicianId = job.AssignedTechnicianId.Value, TechnicianUserId = job.AssignedTechnicianUserId.Value, Rating = request.Rating, Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(), CreatedAtUtc = now };
        var sentiment = await sentimentAnalyzer.AnalyzeAsync(request.Rating, request.Comment, cancellationToken);
        var analysis = new ReviewAnalysis { ReviewId = review.Id, Sentiment = sentiment.Sentiment, Confidence = sentiment.Confidence, KeywordsJson = JsonSerializer.Serialize(sentiment.Keywords), RequiresAdminAttention = sentiment.RequiresAdminAttention, RawAiResponseJson = sentiment.RawResponseJson, CreatedAtUtc = now };
        await repository.AddReviewAsync(review, cancellationToken);
        await repository.AddAnalysisAsync(analysis, cancellationToken);
        await RecalculateRatingAsync(review.TechnicianId, now, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await NotifyReviewAsync(review, analysis, cancellationToken);
        return Result.Success(Map(review, analysis));
    }

    public async Task<Result<ReviewResponse>> GetForJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var review = await repository.GetReviewByJobAsync(jobId, cancellationToken);
        if (review is null) return Result.Failure<ReviewResponse>("review_not_found", "Review was not found.");
        if (review.CustomerUserId != currentUserContext.UserId && review.TechnicianUserId != currentUserContext.UserId) return Result.Failure<ReviewResponse>("forbidden", "You cannot access this review.");
        return Result.Success(Map(review, await repository.GetAnalysisByReviewAsync(review.Id, cancellationToken)));
    }

    public async Task<Result<IReadOnlyCollection<ReviewResponse>>> GetMyTechnicianReviewsAsync(CancellationToken cancellationToken = default)
    {
        var technicianId = await technicianLookupService.GetTechnicianIdByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (!technicianId.HasValue) return Result.Failure<IReadOnlyCollection<ReviewResponse>>("technician_not_found", "Technician profile was not found.");
        return Result.Success<IReadOnlyCollection<ReviewResponse>>(await MapQuery(repository.QueryReviews().Where(review => review.TechnicianId == technicianId.Value), cancellationToken));
    }

    public async Task<Result<TechnicianRatingSummaryResponse>> GetMyTechnicianRatingSummaryAsync(CancellationToken cancellationToken = default)
    {
        var technicianId = await technicianLookupService.GetTechnicianIdByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (!technicianId.HasValue) return Result.Failure<TechnicianRatingSummaryResponse>("technician_not_found", "Technician profile was not found.");
        var snapshot = await repository.GetRatingSnapshotAsync(technicianId.Value, cancellationToken);
        return Result.Success(snapshot is null ? new TechnicianRatingSummaryResponse(technicianId.Value, 0, 0, null) : Map(snapshot));
    }

    public async Task<Result<IReadOnlyCollection<ReviewResponse>>> GetTechnicianReviewsAsync(Guid technicianId, CancellationToken cancellationToken = default) =>
        Result.Success<IReadOnlyCollection<ReviewResponse>>(await MapQuery(repository.QueryReviews().Where(review => review.TechnicianId == technicianId && review.IsVisible), cancellationToken));

    public async Task<Result<IReadOnlyCollection<ReviewResponse>>> GetReviewsAsync(ReviewQuery query, CancellationToken cancellationToken = default)
    {
        var reviews = repository.QueryReviews();
        if (query.TechnicianId.HasValue) reviews = reviews.Where(review => review.TechnicianId == query.TechnicianId);
        if (query.JobId.HasValue) reviews = reviews.Where(review => review.JobId == query.JobId);
        if (query.IsVisible.HasValue) reviews = reviews.Where(review => review.IsVisible == query.IsVisible);
        return Result.Success<IReadOnlyCollection<ReviewResponse>>(await MapQuery(reviews, cancellationToken));
    }

    public async Task<Result<ReviewResponse>> SetVisibilityAsync(Guid id, bool isVisible, CancellationToken cancellationToken = default)
    {
        var review = await repository.GetReviewAsync(id, cancellationToken);
        if (review is null) return Result.Failure<ReviewResponse>("review_not_found", "Review was not found.");
        review.IsVisible = isVisible;
        review.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(review, await repository.GetAnalysisByReviewAsync(review.Id, cancellationToken)));
    }

    private async Task RecalculateRatingAsync(Guid technicianId, DateTime now, CancellationToken cancellationToken)
    {
        var stats = await repository.QueryReviews().Where(review => review.TechnicianId == technicianId && review.IsVisible).GroupBy(review => review.TechnicianId).Select(group => new { Average = group.Average(review => review.Rating), Total = group.Count(), Last = group.Max(review => review.CreatedAtUtc) }).FirstAsync(cancellationToken);
        var snapshot = await repository.GetRatingSnapshotAsync(technicianId, cancellationToken);
        if (snapshot is null)
        {
            snapshot = new TechnicianRatingSnapshot { TechnicianId = technicianId };
            await repository.AddRatingSnapshotAsync(snapshot, cancellationToken);
        }

        snapshot.AverageRating = Math.Round((decimal)stats.Average, 2);
        snapshot.TotalReviews = stats.Total;
        snapshot.LastReviewAtUtc = stats.Last;
        snapshot.UpdatedAtUtc = now;
        await technicianLookupService.UpdateRatingAsync(technicianId, snapshot.AverageRating, snapshot.TotalReviews, cancellationToken);
    }

    private async Task<IReadOnlyCollection<ReviewResponse>> MapQuery(IQueryable<Review> query, CancellationToken cancellationToken)
    {
        var reviews = await query.AsNoTracking().OrderByDescending(review => review.CreatedAtUtc).Take(200).ToListAsync(cancellationToken);
        var ids = reviews.Select(review => review.Id).ToArray();
        var analyses = await repository.QueryAnalysis().AsNoTracking().Where(analysis => ids.Contains(analysis.ReviewId)).ToDictionaryAsync(analysis => analysis.ReviewId, cancellationToken);
        return reviews.Select(review => Map(review, analyses.GetValueOrDefault(review.Id))).ToArray();
    }

    private async Task NotifyReviewAsync(Review review, ReviewAnalysis analysis, CancellationToken cancellationToken)
    {
        await notificationService.SendInAppAsync(review.TechnicianUserId, "review.created", "New review received", "A customer reviewed a completed job.", new Dictionary<string, string?> { ["jobId"] = review.JobId.ToString(), ["reviewId"] = review.Id.ToString() }, cancellationToken);
        if (analysis.RequiresAdminAttention) await notificationService.SendInAppAsync(Guid.Empty, "review.admin_attention", "Review requires attention", "A negative review may require admin attention.", new Dictionary<string, string?> { ["jobId"] = review.JobId.ToString(), ["reviewId"] = review.Id.ToString() }, cancellationToken);
    }

    private static ReviewResponse Map(Review review, ReviewAnalysis? analysis) => new(review.Id, review.JobId, review.CustomerUserId, review.TechnicianId, review.TechnicianUserId, review.Rating, review.Comment, review.IsVisible, review.CreatedAtUtc, analysis is null ? null : new ReviewAnalysisResponse(analysis.Sentiment, analysis.Confidence, analysis.RequiresAdminAttention, analysis.KeywordsJson));
    private static TechnicianRatingSummaryResponse Map(TechnicianRatingSnapshot snapshot) => new(snapshot.TechnicianId, snapshot.AverageRating, snapshot.TotalReviews, snapshot.LastReviewAtUtc);
}
