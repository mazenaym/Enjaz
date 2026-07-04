using Enjaz.Technicians.Domain.Technicians;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Technicians.Infrastructure.Persistence.Configurations;

public sealed class TechnicianSkillConfiguration : IEntityTypeConfiguration<TechnicianSkill>
{
    public void Configure(EntityTypeBuilder<TechnicianSkill> builder)
    {
        builder.ToTable("technician_skills");

        builder.HasKey(skill => skill.Id);
        builder.Property(skill => skill.Id).HasColumnName("id");
        builder.Property(skill => skill.TechnicianId).HasColumnName("technician_id");
        builder.Property(skill => skill.ServiceId).HasColumnName("service_id");
        builder.Property(skill => skill.ServiceCategoryId).HasColumnName("service_category_id");
        builder.Property(skill => skill.SkillLevel).HasColumnName("skill_level").HasMaxLength(50);
        builder.Property(skill => skill.CreatedAtUtc).HasColumnName("created_at_utc");

        builder.HasOne(skill => skill.Technician)
            .WithMany(profile => profile.Skills)
            .HasForeignKey(skill => skill.TechnicianId);

        builder.HasIndex(skill => new { skill.TechnicianId, skill.ServiceId }).IsUnique();
        builder.HasIndex(skill => skill.ServiceId);
        builder.HasIndex(skill => skill.ServiceCategoryId);
    }
}
