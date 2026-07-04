using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Notifications.Infrastructure.Persistence.Configurations;

public sealed class PushDeviceTokenConfiguration : IEntityTypeConfiguration<PushDeviceToken>
{
    public void Configure(EntityTypeBuilder<PushDeviceToken> builder)
    {
        builder.ToTable("push_device_tokens");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnName("id");
        builder.Property(item => item.UserId).HasColumnName("user_id");
        builder.Property(item => item.Token).HasColumnName("token").HasMaxLength(500).IsRequired();
        builder.Property(item => item.Platform).HasColumnName("platform").HasMaxLength(20);
        builder.Property(item => item.Provider).HasColumnName("provider").HasMaxLength(40).IsRequired();
        builder.Property(item => item.DeviceId).HasColumnName("device_id").HasMaxLength(120);
        builder.Property(item => item.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(item => item.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.Property(item => item.LastSeenAtUtc).HasColumnName("last_seen_at_utc");
        builder.HasIndex(item => item.UserId);
        builder.HasIndex(item => item.Token).IsUnique();
    }
}
