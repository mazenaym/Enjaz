using Enjaz.Reviews.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Reviews.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(review => review.Id);
        builder.Property(review => review.Id).HasColumnName("id");
        builder.Property(review => review.JobId).HasColumnName("job_id");
        builder.Property(review => review.CustomerUserId).HasColumnName("customer_user_id");
        builder.Property(review => review.TechnicianId).HasColumnName("technician_id");
        builder.Property(review => review.TechnicianUserId).HasColumnName("technician_user_id");
        builder.Property(review => review.Rating).HasColumnName("rating");
        builder.Property(review => review.Comment).HasColumnName("comment").HasMaxLength(2000);
        builder.Property(review => review.IsVisible).HasColumnName("is_visible").HasDefaultValue(true);
        builder.Property(review => review.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(review => review.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(review => review.JobId).IsUnique();
        builder.HasIndex(review => review.TechnicianId);
        builder.HasIndex(review => review.CustomerUserId);
    }
}
