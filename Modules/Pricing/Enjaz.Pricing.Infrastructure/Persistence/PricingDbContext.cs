using Enjaz.Pricing.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Pricing.Infrastructure.Persistence;

public sealed class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options)
        : base(options)
    {
    }

    public DbSet<PricingRule> PricingRules => Set<PricingRule>();
    public DbSet<CommissionSetting> CommissionSettings => Set<CommissionSetting>();
    public DbSet<VatSetting> VatSettings => Set<VatSetting>();
    public DbSet<DepositRule> DepositRules => Set<DepositRule>();
    public DbSet<PriceSnapshot> PriceSnapshots => Set<PriceSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pricing");
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
