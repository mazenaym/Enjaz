using Enjaz.Maps.Domain.Maps;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Maps.Infrastructure.Persistence;

public sealed class MapsDbContext : DbContext
{
    public MapsDbContext(DbContextOptions<MapsDbContext> options)
        : base(options)
    {
    }

    public DbSet<ServiceZone> ServiceZones => Set<ServiceZone>();

    public DbSet<LocationCalculationCache> LocationCalculationCache => Set<LocationCalculationCache>();

    public DbSet<TechnicianLocationSnapshot> TechnicianLocationSnapshots => Set<TechnicianLocationSnapshot>();

    public DbSet<TechnicianLocationHistory> TechnicianLocationHistory => Set<TechnicianLocationHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("maps");
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
