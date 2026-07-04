using Enjaz.Payments.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Payments.Infrastructure.Persistence.Configurations;

public sealed class RefundRequestConfiguration : IEntityTypeConfiguration<RefundRequest>
{
    public void Configure(EntityTypeBuilder<RefundRequest> builder)
    {
        builder.ToTable("refund_requests");
        builder.HasKey(request => request.Id);
        builder.Property(request => request.Id).HasColumnName("id");
        builder.Property(request => request.PaymentId).HasColumnName("payment_id");
        builder.Property(request => request.JobId).HasColumnName("job_id");
        builder.Property(request => request.Amount).HasColumnName("amount").HasPrecision(18, 2);
        builder.Property(request => request.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(request => request.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();
        builder.Property(request => request.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(request => request.RequestedByUserId).HasColumnName("requested_by_user_id");
        builder.Property(request => request.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(request => request.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(request => request.PaymentId);
        builder.HasIndex(request => request.JobId);
    }
}
