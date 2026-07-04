using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Notifications.Infrastructure.Persistence.Configurations;

public sealed class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("notification_templates");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnName("id");
        builder.Property(item => item.Type).HasColumnName("type").HasMaxLength(80).IsRequired();
        builder.Property(item => item.Channel).HasColumnName("channel").HasMaxLength(20).IsRequired();
        builder.Property(item => item.Language).HasColumnName("language").HasMaxLength(10).HasDefaultValue("ar");
        builder.Property(item => item.TitleTemplate).HasColumnName("title_template").HasMaxLength(200).IsRequired();
        builder.Property(item => item.BodyTemplate).HasColumnName("body_template").HasMaxLength(2000).IsRequired();
        builder.Property(item => item.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(item => item.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(item => new { item.Type, item.Channel, item.Language });
        builder.HasIndex(item => new { item.Type, item.Channel, item.Language }).IsUnique().HasFilter("is_active = true");
        builder.HasData(NotificationTemplateSeed.Templates);
    }
}
