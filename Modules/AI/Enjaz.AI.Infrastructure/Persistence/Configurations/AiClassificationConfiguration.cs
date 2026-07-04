using Enjaz.AI.Domain.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.AI.Infrastructure.Persistence.Configurations;

public sealed class AiClassificationConfiguration : IEntityTypeConfiguration<AiClassification>
{
    public void Configure(EntityTypeBuilder<AiClassification> builder)
    {
        builder.ToTable("ai_classifications");

        builder.HasKey(classification => classification.Id);
        builder.Property(classification => classification.Id).HasColumnName("id");
        builder.Property(classification => classification.AiRequestId).HasColumnName("ai_request_id");
        builder.Property(classification => classification.UserId).HasColumnName("user_id");
        builder.Property(classification => classification.ServiceId).HasColumnName("service_id");
        builder.Property(classification => classification.ServiceCategoryId).HasColumnName("service_category_id");
        builder.Property(classification => classification.CustomerDescription).HasColumnName("customer_description").IsRequired();
        builder.Property(classification => classification.ComplexityId).HasColumnName("complexity_id");
        builder.Property(classification => classification.ComplexityName).HasColumnName("complexity_name").HasMaxLength(50).IsRequired();
        builder.Property(classification => classification.Confidence).HasColumnName("confidence").HasPrecision(5, 2);
        builder.Property(classification => classification.SuggestedAction).HasColumnName("suggested_action").HasMaxLength(1000);
        builder.Property(classification => classification.RequiresInspection).HasColumnName("requires_inspection");
        builder.Property(classification => classification.CreatedAtUtc).HasColumnName("created_at_utc");

        builder.HasIndex(classification => classification.ServiceId);
        builder.HasIndex(classification => classification.ServiceCategoryId);
        builder.HasIndex(classification => classification.CreatedAtUtc);
    }
}
