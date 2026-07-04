using Enjaz.Support.Domain.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Support.Infrastructure.Persistence.Configurations;

public sealed class SupportTicketMessageConfiguration : IEntityTypeConfiguration<SupportTicketMessage>
{
    public void Configure(EntityTypeBuilder<SupportTicketMessage> builder)
    {
        builder.ToTable("support_ticket_messages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).HasColumnName("id");
        builder.Property(message => message.TicketId).HasColumnName("ticket_id");
        builder.Property(message => message.SenderUserId).HasColumnName("sender_user_id");
        builder.Property(message => message.SenderRole).HasColumnName("sender_role").HasMaxLength(50).IsRequired();
        builder.Property(message => message.Message).HasColumnName("message").HasMaxLength(4000).IsRequired();
        builder.Property(message => message.IsInternal).HasColumnName("is_internal").HasDefaultValue(false);
        builder.Property(message => message.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(message => message.TicketId);
        builder.HasIndex(message => message.SenderUserId);
    }
}
