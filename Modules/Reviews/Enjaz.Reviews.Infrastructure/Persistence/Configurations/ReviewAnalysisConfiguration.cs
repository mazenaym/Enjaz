using Enjaz.Reviews.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Reviews.Infrastructure.Persistence.Configurations;

public sealed class ReviewAnalysisConfiguration : IEntityTypeConfiguration<ReviewAnalysis>
{
    public void Configure(EntityTypeBuilder<ReviewAnalysis> builder)
    {
        builder.ToTable("review_analysis");
        builder.HasKey(analysis => analysis.Id);
        builder.Property(analysis => analysis.Id).HasColumnName("id");
        builder.Property(analysis => analysis.ReviewId).HasColumnName("review_id");
        builder.Property(analysis => analysis.Sentiment).HasColumnName("sentiment").HasMaxLength(50).IsRequired();
        builder.Property(analysis => analysis.Confidence).HasColumnName("confidence").HasPrecision(5, 4);
        builder.Property(analysis => analysis.KeywordsJson).HasColumnName("keywords_json");
        builder.Property(analysis => analysis.RequiresAdminAttention).HasColumnName("requires_admin_attention").HasDefaultValue(false);
        builder.Property(analysis => analysis.RawAiResponseJson).HasColumnName("raw_ai_response_json");
        builder.Property(analysis => analysis.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(analysis => analysis.ReviewId).IsUnique();
        builder.HasIndex(analysis => analysis.RequiresAdminAttention);
    }
}
