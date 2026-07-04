using Enjaz.Technicians.Domain.Technicians;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Technicians.Infrastructure.Persistence.Configurations;

public sealed class TechnicianProfileConfiguration : IEntityTypeConfiguration<TechnicianProfile>
{
    public void Configure(EntityTypeBuilder<TechnicianProfile> builder)
    {
        builder.ToTable("technician_profiles");

        builder.HasKey(profile => profile.Id);
        builder.Property(profile => profile.Id).HasColumnName("id");
        builder.Property(profile => profile.UserId).HasColumnName("user_id");
        builder.Property(profile => profile.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();
        builder.Property(profile => profile.PhoneNumber).HasColumnName("phone_number").HasMaxLength(32).IsRequired();
        builder.Property(profile => profile.Email).HasColumnName("email").HasMaxLength(320);
        builder.Property(profile => profile.NationalId).HasColumnName("national_id").HasMaxLength(100);
        builder.Property(profile => profile.ProfileImageUrl).HasColumnName("profile_image_url").HasMaxLength(1000);
        builder.Property(profile => profile.Bio).HasColumnName("bio").HasMaxLength(2000);
        builder.Property(profile => profile.YearsOfExperience).HasColumnName("years_of_experience");
        builder.Property(profile => profile.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(profile => profile.AvailabilityStatus).HasColumnName("availability_status").HasMaxLength(50).IsRequired();
        builder.Property(profile => profile.AverageRating).HasColumnName("average_rating").HasPrecision(5, 2).HasDefaultValue(0m);
        builder.Property(profile => profile.TotalReviews).HasColumnName("total_reviews").HasDefaultValue(0);
        builder.Property(profile => profile.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(profile => profile.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.Property(profile => profile.ApprovedAtUtc).HasColumnName("approved_at_utc");
        builder.Property(profile => profile.RejectedAtUtc).HasColumnName("rejected_at_utc");
        builder.Property(profile => profile.RejectionReason).HasColumnName("rejection_reason").HasMaxLength(1000);

        builder.HasIndex(profile => profile.UserId).IsUnique();
        builder.HasIndex(profile => profile.Status);
        builder.HasIndex(profile => profile.AvailabilityStatus);
    }
}
