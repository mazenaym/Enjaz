using Enjaz.Reviews.Application.Reviews;
using Enjaz.Reviews.Domain.Reviews;
using Enjaz.Reviews.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Reviews.Infrastructure.Reviews;

public sealed class ReviewsRepository(ReviewsDbContext dbContext) : IReviewsRepository
{
    public IQueryable<Review> QueryReviews() => dbContext.Reviews;
    public IQueryable<ReviewAnalysis> QueryAnalysis() => dbContext.ReviewAnalysis;
    public IQueryable<TechnicianRatingSnapshot> QueryRatingSnapshots() => dbContext.TechnicianRatingSnapshots;
    public Task<Review?> GetReviewAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.Reviews.FirstOrDefaultAsync(review => review.Id == id, cancellationToken);
    public Task<Review?> GetReviewByJobAsync(Guid jobId, CancellationToken cancellationToken = default) => dbContext.Reviews.FirstOrDefaultAsync(review => review.JobId == jobId, cancellationToken);
    public Task<ReviewAnalysis?> GetAnalysisByReviewAsync(Guid reviewId, CancellationToken cancellationToken = default) => dbContext.ReviewAnalysis.FirstOrDefaultAsync(analysis => analysis.ReviewId == reviewId, cancellationToken);
    public Task<TechnicianRatingSnapshot?> GetRatingSnapshotAsync(Guid technicianId, CancellationToken cancellationToken = default) => dbContext.TechnicianRatingSnapshots.FirstOrDefaultAsync(snapshot => snapshot.TechnicianId == technicianId, cancellationToken);
    public async Task AddReviewAsync(Review review, CancellationToken cancellationToken = default) => await dbContext.Reviews.AddAsync(review, cancellationToken);
    public async Task AddAnalysisAsync(ReviewAnalysis analysis, CancellationToken cancellationToken = default) => await dbContext.ReviewAnalysis.AddAsync(analysis, cancellationToken);
    public async Task AddRatingSnapshotAsync(TechnicianRatingSnapshot snapshot, CancellationToken cancellationToken = default) => await dbContext.TechnicianRatingSnapshots.AddAsync(snapshot, cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
