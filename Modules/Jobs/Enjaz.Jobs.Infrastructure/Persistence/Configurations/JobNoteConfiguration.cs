using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Jobs.Infrastructure.Persistence.Configurations;

public sealed class JobNoteConfiguration : IEntityTypeConfiguration<JobNote>
{
    public void Configure(EntityTypeBuilder<JobNote> builder)
    {
        builder.ToTable("job_notes");
        builder.HasKey(note => note.Id);
        builder.Property(note => note.Id).HasColumnName("id");
        builder.Property(note => note.JobId).HasColumnName("job_id").IsRequired();
        builder.Property(note => note.AuthorUserId).HasColumnName("author_user_id").IsRequired();
        builder.Property(note => note.AuthorRole).HasColumnName("author_role").HasMaxLength(32).IsRequired();
        builder.Property(note => note.NoteType).HasColumnName("note_type").HasMaxLength(32).IsRequired();
        builder.Property(note => note.Text).HasColumnName("text").HasMaxLength(2000).IsRequired();
        builder.Property(note => note.IsInternal).HasColumnName("is_internal").HasDefaultValue(false);
        builder.Property(note => note.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(note => note.JobId);
    }
}
