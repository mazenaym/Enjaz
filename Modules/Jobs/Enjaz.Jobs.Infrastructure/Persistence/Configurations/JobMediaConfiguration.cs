using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Jobs.Infrastructure.Persistence.Configurations;

public sealed class JobMediaConfiguration : IEntityTypeConfiguration<JobMedia>
{
    public void Configure(EntityTypeBuilder<JobMedia> builder)
    {
        builder.ToTable("job_media");
        builder.HasKey(media => media.Id);
        builder.Property(media => media.Id).HasColumnName("id");
        builder.Property(media => media.JobId).HasColumnName("job_id").IsRequired();
        builder.Property(media => media.MediaType).HasColumnName("media_type").HasMaxLength(32).IsRequired();
        builder.Property(media => media.FileUrl).HasColumnName("file_url").HasMaxLength(1000).IsRequired();
        builder.Property(media => media.FileKey).HasColumnName("file_key").HasMaxLength(500);
        builder.Property(media => media.Caption).HasColumnName("caption").HasMaxLength(500);
        builder.Property(media => media.UploadedByUserId).HasColumnName("uploaded_by_user_id").IsRequired();
        builder.Property(media => media.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(media => media.JobId);
    }
}
