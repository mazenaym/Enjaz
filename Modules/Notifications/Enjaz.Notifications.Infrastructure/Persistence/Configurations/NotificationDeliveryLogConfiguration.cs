using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Notifications.Infrastructure.Persistence.Configurations;

public sealed class NotificationDeliveryLogConfiguration : IEntityTypeConfiguration<NotificationDeliveryLog>
{
    public void Configure(EntityTypeBuilder<NotificationDeliveryLog> builder)
    {
        builder.ToTable("notification_delivery_logs");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnName("id");
        builder.Property(item => item.NotificationId).HasColumnName("notification_id");
        builder.Property(item => item.UserId).HasColumnName("user_id");
        builder.Property(item => item.Channel).HasColumnName("channel").HasMaxLength(20).IsRequired();
        builder.Property(item => item.Provider).HasColumnName("provider").HasMaxLength(40).IsRequired();
        builder.Property(item => item.Recipient).HasColumnName("recipient").HasMaxLength(500);
        builder.Property(item => item.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(item => item.AttemptCount).HasColumnName("attempt_count").HasDefaultValue(0);
        builder.Property(item => item.LastAttemptAtUtc).HasColumnName("last_attempt_at_utc");
        builder.Property(item => item.NextRetryAtUtc).HasColumnName("next_retry_at_utc");
        builder.Property(item => item.ErrorMessage).HasColumnName("error_message").HasMaxLength(1000);
        builder.Property(item => item.RequestJson).HasColumnName("request_json").HasColumnType("jsonb");
        builder.Property(item => item.ResponseJson).HasColumnName("response_json").HasColumnType("jsonb");
        builder.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(item => item.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => item.NextRetryAtUtc);
    }
}
