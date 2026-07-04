using Enjaz.AI.Domain.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.AI.Infrastructure.Persistence.Configurations;

public sealed class AiRequestConfiguration : IEntityTypeConfiguration<AiRequest>
{
    public void Configure(EntityTypeBuilder<AiRequest> builder)
    {
        builder.ToTable("ai_requests");

        builder.HasKey(request => request.Id);
        builder.Property(request => request.Id).HasColumnName("id");
        builder.Property(request => request.UserId).HasColumnName("user_id");
        builder.Property(request => request.Feature).HasColumnName("feature").HasMaxLength(100).IsRequired();
        builder.Property(request => request.Provider).HasColumnName("provider").HasMaxLength(50).IsRequired();
        builder.Property(request => request.Model).HasColumnName("model").HasMaxLength(100);
        builder.Property(request => request.InputText).HasColumnName("input_text").IsRequired();
        builder.Property(request => request.InputJson).HasColumnName("input_json").HasColumnType("jsonb");
        builder.Property(request => request.RawResponseJson).HasColumnName("raw_response_json").HasColumnType("jsonb");
        builder.Property(request => request.Success).HasColumnName("success");
        builder.Property(request => request.ErrorMessage).HasColumnName("error_message").HasMaxLength(1000);
        builder.Property(request => request.CreatedAtUtc).HasColumnName("created_at_utc");

        builder.HasIndex(request => request.Feature);
        builder.HasIndex(request => request.CreatedAtUtc);
    }
}
