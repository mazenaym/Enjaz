using Enjaz.Reviews.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Reviews.Infrastructure.Persistence;

public sealed class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options) : DbContext(options)
{
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewAnalysis> ReviewAnalysis => Set<ReviewAnalysis>();
    public DbSet<TechnicianRatingSnapshot> TechnicianRatingSnapshots => Set<TechnicianRatingSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reviews");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReviewsDbContext).Assembly);
    }
}
