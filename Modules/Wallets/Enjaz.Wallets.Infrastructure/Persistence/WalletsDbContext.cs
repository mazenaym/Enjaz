using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Wallets.Infrastructure.Persistence;

public sealed class WalletsDbContext(DbContextOptions<WalletsDbContext> options) : DbContext(options)
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<LedgerTransaction> LedgerTransactions => Set<LedgerTransaction>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<PlatformEarning> PlatformEarnings => Set<PlatformEarning>();
    public DbSet<TechnicianEarning> TechnicianEarnings => Set<TechnicianEarning>();
    public DbSet<PayoutBatch> PayoutBatches => Set<PayoutBatch>();
    public DbSet<PayoutBatchItem> PayoutBatchItems => Set<PayoutBatchItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("wallets");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletsDbContext).Assembly);
    }
}
