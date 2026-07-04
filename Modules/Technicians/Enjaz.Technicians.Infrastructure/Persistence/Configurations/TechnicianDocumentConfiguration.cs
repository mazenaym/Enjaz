using Enjaz.Technicians.Domain.Technicians;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Technicians.Infrastructure.Persistence.Configurations;

public sealed class TechnicianDocumentConfiguration : IEntityTypeConfiguration<TechnicianDocument>
{
    public void Configure(EntityTypeBuilder<TechnicianDocument> builder)
    {
        builder.ToTable("technician_documents");

        builder.HasKey(document => document.Id);
        builder.Property(document => document.Id).HasColumnName("id");
        builder.Property(document => document.TechnicianId).HasColumnName("technician_id");
        builder.Property(document => document.DocumentType).HasColumnName("document_type").HasMaxLength(100).IsRequired();
        builder.Property(document => document.FileUrl).HasColumnName("file_url").HasMaxLength(1000).IsRequired();
        builder.Property(document => document.FileKey).HasColumnName("file_key").HasMaxLength(500);
        builder.Property(document => document.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(document => document.RejectionReason).HasColumnName("rejection_reason").HasMaxLength(1000);
        builder.Property(document => document.UploadedAtUtc).HasColumnName("uploaded_at_utc");
        builder.Property(document => document.ReviewedAtUtc).HasColumnName("reviewed_at_utc");

        builder.HasOne(document => document.Technician)
            .WithMany(profile => profile.Documents)
            .HasForeignKey(document => document.TechnicianId);

        builder.HasIndex(document => document.TechnicianId);
    }
}
