using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Jobs.Infrastructure.Persistence.Configurations;

public sealed class JobCounterConfiguration : IEntityTypeConfiguration<JobCounter>
{
    public void Configure(EntityTypeBuilder<JobCounter> builder)
    {
        builder.ToTable("job_counters");
        builder.HasKey(counter => counter.Id);
        builder.Property(counter => counter.Id).HasColumnName("id");
        builder.Property(counter => counter.YearMonth).HasColumnName("year_month").HasMaxLength(6).IsRequired();
        builder.Property(counter => counter.LastNumber).HasColumnName("last_number").IsRequired();
        builder.Property(counter => counter.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(counter => counter.YearMonth).IsUnique();
    }
}
