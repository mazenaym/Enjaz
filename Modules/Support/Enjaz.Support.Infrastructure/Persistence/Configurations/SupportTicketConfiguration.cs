using Enjaz.Support.Domain.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Support.Infrastructure.Persistence.Configurations;

public sealed class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("support_tickets");
        builder.HasKey(ticket => ticket.Id);
        builder.Property(ticket => ticket.Id).HasColumnName("id");
        builder.Property(ticket => ticket.TicketNumber).HasColumnName("ticket_number").HasMaxLength(40).IsRequired();
        builder.Property(ticket => ticket.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(ticket => ticket.RelatedJobId).HasColumnName("related_job_id");
        builder.Property(ticket => ticket.Category).HasColumnName("category").HasMaxLength(50).IsRequired();
        builder.Property(ticket => ticket.Priority).HasColumnName("priority").HasMaxLength(50).IsRequired();
        builder.Property(ticket => ticket.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(ticket => ticket.Subject).HasColumnName("subject").HasMaxLength(300).IsRequired();
        builder.Property(ticket => ticket.Description).HasColumnName("description").HasMaxLength(4000).IsRequired();
        builder.Property(ticket => ticket.AssignedAdminUserId).HasColumnName("assigned_admin_user_id");
        builder.Property(ticket => ticket.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(ticket => ticket.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.Property(ticket => ticket.ClosedAtUtc).HasColumnName("closed_at_utc");
        builder.HasIndex(ticket => ticket.TicketNumber).IsUnique();
        builder.HasIndex(ticket => ticket.CreatedByUserId);
        builder.HasIndex(ticket => ticket.Status);
        builder.HasIndex(ticket => ticket.Priority);
    }
}
