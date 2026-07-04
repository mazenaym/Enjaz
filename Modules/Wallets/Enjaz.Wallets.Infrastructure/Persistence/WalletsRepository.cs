using Enjaz.Wallets.Application.Wallets;
using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Wallets.Infrastructure.Persistence;

public sealed class WalletsRepository(WalletsDbContext dbContext) : IWalletsRepository
{
    public IQueryable<Wallet> QueryWallets() => dbContext.Wallets;
    public IQueryable<LedgerTransaction> QueryLedgerTransactions() => dbContext.LedgerTransactions;
    public IQueryable<LedgerEntry> QueryLedgerEntries() => dbContext.LedgerEntries;
    public IQueryable<PlatformEarning> QueryPlatformEarnings() => dbContext.PlatformEarnings;
    public IQueryable<TechnicianEarning> QueryTechnicianEarnings() => dbContext.TechnicianEarnings;
    public IQueryable<PayoutBatch> QueryPayoutBatches() => dbContext.PayoutBatches;
    public IQueryable<PayoutBatchItem> QueryPayoutBatchItems() => dbContext.PayoutBatchItems;

    public Task<Wallet?> GetWalletAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.Wallets.FirstOrDefaultAsync(wallet => wallet.Id == id, cancellationToken);

    public Task<Wallet?> GetActiveWalletAsync(string ownerType, Guid? ownerUserId, Guid? technicianId, string currency, CancellationToken cancellationToken = default)
    {
        var normalizedCurrency = NormalizeCurrency(currency);
        return dbContext.Wallets.FirstOrDefaultAsync(wallet =>
            wallet.IsActive &&
            wallet.OwnerType == ownerType &&
            wallet.Currency == normalizedCurrency &&
            wallet.OwnerUserId == ownerUserId &&
            wallet.TechnicianId == technicianId, cancellationToken);
    }

    public async Task<Wallet> EnsureWalletAsync(string ownerType, Guid? ownerUserId, Guid? technicianId, string currency, DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        var normalizedCurrency = NormalizeCurrency(currency);
        var wallet = await GetActiveWalletAsync(ownerType, ownerUserId, technicianId, normalizedCurrency, cancellationToken);
        if (wallet is not null) return wallet;

        wallet = new Wallet
        {
            OwnerType = ownerType,
            OwnerUserId = ownerUserId,
            TechnicianId = technicianId,
            Currency = normalizedCurrency,
            IsActive = true,
            CreatedAtUtc = nowUtc
        };
        await dbContext.Wallets.AddAsync(wallet, cancellationToken);
        return wallet;
    }

    public Task<LedgerTransaction?> GetLedgerTransactionByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return dbContext.LedgerTransactions.FirstOrDefaultAsync(transaction => transaction.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task AddLedgerTransactionAsync(LedgerTransaction transaction, CancellationToken cancellationToken = default) => await dbContext.LedgerTransactions.AddAsync(transaction, cancellationToken);
    public async Task AddLedgerEntryAsync(LedgerEntry entry, CancellationToken cancellationToken = default) => await dbContext.LedgerEntries.AddAsync(entry, cancellationToken);
    public async Task AddPlatformEarningAsync(PlatformEarning earning, CancellationToken cancellationToken = default) => await dbContext.PlatformEarnings.AddAsync(earning, cancellationToken);
    public async Task AddTechnicianEarningAsync(TechnicianEarning earning, CancellationToken cancellationToken = default) => await dbContext.TechnicianEarnings.AddAsync(earning, cancellationToken);
    public async Task AddPayoutBatchAsync(PayoutBatch batch, CancellationToken cancellationToken = default) => await dbContext.PayoutBatches.AddAsync(batch, cancellationToken);
    public async Task AddPayoutBatchItemAsync(PayoutBatchItem item, CancellationToken cancellationToken = default) => await dbContext.PayoutBatchItems.AddAsync(item, cancellationToken);

    public async Task<string> GenerateTransactionNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        var prefix = $"LT-{nowUtc:yyyyMMdd}";
        var count = await dbContext.LedgerTransactions.CountAsync(transaction => transaction.TransactionNumber.StartsWith(prefix), cancellationToken);
        return $"{prefix}-{count + 1:000000}";
    }

    public async Task<string> GenerateBatchNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        var prefix = $"PB-{nowUtc:yyyyMMdd}";
        var count = await dbContext.PayoutBatches.CountAsync(batch => batch.BatchNumber.StartsWith(prefix), cancellationToken);
        return $"{prefix}-{count + 1:000000}";
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var result = await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        });
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);

    private static string NormalizeCurrency(string currency) => string.IsNullOrWhiteSpace(currency) ? "EGP" : currency.Trim().ToUpperInvariant();
}
