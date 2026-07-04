using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Wallets.Infrastructure.Persistence.Configurations;

public sealed class TechnicianEarningConfiguration : IEntityTypeConfiguration<TechnicianEarning>
{
    public void Configure(EntityTypeBuilder<TechnicianEarning> builder)
    {
        builder.ToTable("technician_earnings");
        builder.HasKey(earning => earning.Id);
        builder.Property(earning => earning.Id).HasColumnName("id");
        builder.Property(earning => earning.JobId).HasColumnName("job_id");
        builder.Property(earning => earning.PaymentId).HasColumnName("payment_id");
        builder.Property(earning => earning.TechnicianId).HasColumnName("technician_id");
        builder.Property(earning => earning.TechnicianUserId).HasColumnName("technician_user_id");
        builder.Property(earning => earning.Amount).HasColumnName("amount").HasPrecision(18, 2);
        builder.Property(earning => earning.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP");
        builder.Property(earning => earning.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(earning => earning.AvailableAtUtc).HasColumnName("available_at_utc");
        builder.Property(earning => earning.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(earning => earning.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(earning => earning.JobId);
        builder.HasIndex(earning => earning.TechnicianId);
        builder.HasIndex(earning => earning.Status);
    }
}
