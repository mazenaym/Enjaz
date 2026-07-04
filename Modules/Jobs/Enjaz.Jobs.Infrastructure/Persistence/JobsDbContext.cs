using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Infrastructure.Persistence;

public sealed class JobsDbContext(DbContextOptions<JobsDbContext> options) : DbContext(options)
{
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobStatusHistory> JobStatusHistory => Set<JobStatusHistory>();
    public DbSet<JobMedia> JobMedia => Set<JobMedia>();
    public DbSet<JobNote> JobNotes => Set<JobNote>();
    public DbSet<JobAssignment> JobAssignments => Set<JobAssignment>();
    public DbSet<JobCounter> JobCounters => Set<JobCounter>();
    public DbSet<JobOperationAlert> JobOperationAlerts => Set<JobOperationAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("jobs");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobsDbContext).Assembly);
    }
}
