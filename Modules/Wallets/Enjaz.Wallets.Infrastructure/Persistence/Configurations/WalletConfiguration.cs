using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Wallets.Infrastructure.Persistence.Configurations;

public sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");
        builder.HasKey(wallet => wallet.Id);
        builder.Property(wallet => wallet.Id).HasColumnName("id");
        builder.Property(wallet => wallet.OwnerType).HasColumnName("owner_type").HasMaxLength(40).IsRequired();
        builder.Property(wallet => wallet.OwnerUserId).HasColumnName("owner_user_id");
        builder.Property(wallet => wallet.TechnicianId).HasColumnName("technician_id");
        builder.Property(wallet => wallet.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP").IsRequired();
        builder.Property(wallet => wallet.AvailableBalance).HasColumnName("available_balance").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(wallet => wallet.PendingBalance).HasColumnName("pending_balance").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(wallet => wallet.TotalCredited).HasColumnName("total_credited").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(wallet => wallet.TotalDebited).HasColumnName("total_debited").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(wallet => wallet.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(wallet => wallet.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(wallet => wallet.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(wallet => new { wallet.OwnerType, wallet.OwnerUserId, wallet.Currency })
            .IsUnique()
            .HasFilter("is_active = true AND owner_user_id IS NOT NULL AND technician_id IS NULL");
        builder.HasIndex(wallet => new { wallet.OwnerType, wallet.TechnicianId, wallet.Currency })
            .IsUnique()
            .HasFilter("is_active = true AND technician_id IS NOT NULL");
        builder.HasIndex(wallet => new { wallet.OwnerType, wallet.Currency })
            .IsUnique()
            .HasFilter("is_active = true AND owner_user_id IS NULL AND technician_id IS NULL");
        builder.HasIndex(wallet => wallet.TechnicianId);
    }
}
