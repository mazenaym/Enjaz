using Enjaz.Reviews.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Reviews.Infrastructure.Persistence.Configurations;

public sealed class TechnicianRatingSnapshotConfiguration : IEntityTypeConfiguration<TechnicianRatingSnapshot>
{
    public void Configure(EntityTypeBuilder<TechnicianRatingSnapshot> builder)
    {
        builder.ToTable("technician_rating_snapshots");
        builder.HasKey(snapshot => snapshot.Id);
        builder.Property(snapshot => snapshot.Id).HasColumnName("id");
        builder.Property(snapshot => snapshot.TechnicianId).HasColumnName("technician_id");
        builder.Property(snapshot => snapshot.AverageRating).HasColumnName("average_rating").HasPrecision(5, 2);
        builder.Property(snapshot => snapshot.TotalReviews).HasColumnName("total_reviews");
        builder.Property(snapshot => snapshot.LastReviewAtUtc).HasColumnName("last_review_at_utc");
        builder.Property(snapshot => snapshot.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(snapshot => snapshot.TechnicianId).IsUnique();
    }
}
