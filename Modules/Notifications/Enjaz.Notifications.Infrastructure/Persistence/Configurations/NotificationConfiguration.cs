using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Notifications.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnName("id");
        builder.Property(item => item.UserId).HasColumnName("user_id");
        builder.Property(item => item.Channel).HasColumnName("channel").HasMaxLength(20).IsRequired();
        builder.Property(item => item.Type).HasColumnName("type").HasMaxLength(80).IsRequired();
        builder.Property(item => item.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(item => item.Body).HasColumnName("body").HasMaxLength(2000).IsRequired();
        builder.Property(item => item.DataJson).HasColumnName("data_json").HasColumnType("jsonb");
        builder.Property(item => item.IsRead).HasColumnName("is_read").HasDefaultValue(false);
        builder.Property(item => item.ReadAtUtc).HasColumnName("read_at_utc");
        builder.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(item => item.UserId);
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.CreatedAtUtc);
        builder.HasIndex(item => item.IsRead);
    }
}
