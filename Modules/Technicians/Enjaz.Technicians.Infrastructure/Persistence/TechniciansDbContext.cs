using Enjaz.Technicians.Domain.Technicians;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Technicians.Infrastructure.Persistence;

public sealed class TechniciansDbContext : DbContext
{
    public TechniciansDbContext(DbContextOptions<TechniciansDbContext> options)
        : base(options)
    {
    }

    public DbSet<TechnicianProfile> TechnicianProfiles => Set<TechnicianProfile>();

    public DbSet<TechnicianDocument> TechnicianDocuments => Set<TechnicianDocument>();

    public DbSet<TechnicianSkill> TechnicianSkills => Set<TechnicianSkill>();

    public DbSet<TechnicianAvailabilityHistory> TechnicianAvailabilityHistory => Set<TechnicianAvailabilityHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("technicians");
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
