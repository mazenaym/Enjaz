using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Wallets.Application.Wallets;

public sealed class WalletsService(IWalletsRepository repository, ICurrentUserContext currentUserContext)
    : IWalletsService, IAdminWalletsService
{
    public async Task<Result<WalletResponse>> GetMyWalletAsync(CancellationToken cancellationToken = default)
    {
        var wallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Customer, currentUserContext.UserId, null, "EGP", DateTime.UtcNow, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapWallet(wallet));
    }

    public async Task<Result<IReadOnlyCollection<WalletEntryResponse>>> GetMyWalletTransactionsAsync(CancellationToken cancellationToken = default)
    {
        var wallet = await repository.GetActiveWalletAsync(WalletOwnerTypes.Customer, currentUserContext.UserId, null, "EGP", cancellationToken);
        if (wallet is null) return Result.Success<IReadOnlyCollection<WalletEntryResponse>>(Array.Empty<WalletEntryResponse>());
        return Result.Success<IReadOnlyCollection<WalletEntryResponse>>(await GetEntries(wallet.Id).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<WalletResponse>> GetMyTechnicianWalletAsync(CancellationToken cancellationToken = default)
    {
        var wallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Technician, currentUserContext.UserId, null, "EGP", DateTime.UtcNow, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapWallet(wallet));
    }

    public async Task<Result<IReadOnlyCollection<TechnicianEarningResponse>>> GetMyTechnicianEarningsAsync(string? status, CancellationToken cancellationToken = default)
    {
        var query = repository.QueryTechnicianEarnings().AsNoTracking().Where(earning => earning.TechnicianUserId == currentUserContext.UserId);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(earning => earning.Status == status);
        return Result.Success<IReadOnlyCollection<TechnicianEarningResponse>>(await query.OrderByDescending(earning => earning.CreatedAtUtc).Select(earning => MapTechnicianEarning(earning)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<WalletResponse>>> GetWalletsAsync(WalletQuery query, CancellationToken cancellationToken = default)
    {
        var wallets = repository.QueryWallets().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.OwnerType)) wallets = wallets.Where(wallet => wallet.OwnerType == query.OwnerType);
        if (query.OwnerUserId.HasValue) wallets = wallets.Where(wallet => wallet.OwnerUserId == query.OwnerUserId);
        if (!string.IsNullOrWhiteSpace(query.Currency)) wallets = wallets.Where(wallet => wallet.Currency == query.Currency);
        return Result.Success<IReadOnlyCollection<WalletResponse>>(await wallets.OrderBy(wallet => wallet.OwnerType).ThenBy(wallet => wallet.CreatedAtUtc).Select(wallet => MapWallet(wallet)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<WalletResponse>> GetWalletAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var wallet = await repository.GetWalletAsync(id, cancellationToken);
        return wallet is null ? Result.Failure<WalletResponse>("wallet_not_found", "Wallet was not found.") : Result.Success(MapWallet(wallet));
    }

    public async Task<Result<IReadOnlyCollection<LedgerTransactionResponse>>> GetLedgerTransactionsAsync(LedgerTransactionQuery query, CancellationToken cancellationToken = default)
    {
        var transactions = repository.QueryLedgerTransactions().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.SourceModule)) transactions = transactions.Where(transaction => transaction.SourceModule == query.SourceModule);
        if (query.SourceEntityId.HasValue) transactions = transactions.Where(transaction => transaction.SourceEntityId == query.SourceEntityId);
        if (!string.IsNullOrWhiteSpace(query.TransactionType)) transactions = transactions.Where(transaction => transaction.TransactionType == query.TransactionType);
        if (query.FromDateUtc.HasValue) transactions = transactions.Where(transaction => transaction.CreatedAtUtc >= query.FromDateUtc);
        if (query.ToDateUtc.HasValue) transactions = transactions.Where(transaction => transaction.CreatedAtUtc <= query.ToDateUtc);
        return Result.Success<IReadOnlyCollection<LedgerTransactionResponse>>(await transactions.OrderByDescending(transaction => transaction.CreatedAtUtc).Take(500).Select(transaction => MapTransaction(transaction)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<PlatformEarningResponse>>> GetPlatformEarningsAsync(CancellationToken cancellationToken = default)
    {
        return Result.Success<IReadOnlyCollection<PlatformEarningResponse>>(await repository.QueryPlatformEarnings().AsNoTracking().OrderByDescending(earning => earning.CreatedAtUtc).Take(500).Select(earning => MapPlatformEarning(earning)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<TechnicianEarningResponse>>> GetTechnicianEarningsAsync(string? status, CancellationToken cancellationToken = default)
    {
        var query = repository.QueryTechnicianEarnings().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(earning => earning.Status == status);
        return Result.Success<IReadOnlyCollection<TechnicianEarningResponse>>(await query.OrderByDescending(earning => earning.CreatedAtUtc).Take(500).Select(earning => MapTechnicianEarning(earning)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<PayoutBatchResponse>> CreatePayoutBatchAsync(CreatePayoutBatchRequest request, CancellationToken cancellationToken = default)
    {
        var currency = string.IsNullOrWhiteSpace(request.Currency) ? "EGP" : request.Currency.Trim().ToUpperInvariant();
        var technicianIds = request.TechnicianIds.Distinct().ToArray();
        var wallets = await repository.QueryWallets()
            .Where(wallet => wallet.OwnerType == WalletOwnerTypes.Technician && wallet.TechnicianId.HasValue && technicianIds.Contains(wallet.TechnicianId.Value) && wallet.Currency == currency && wallet.AvailableBalance > 0)
            .ToArrayAsync(cancellationToken);

        if (wallets.Length == 0)
        {
            return Result.Failure<PayoutBatchResponse>("no_available_wallet_balances", "No selected technician wallets have available balances.");
        }

        var now = DateTime.UtcNow;
        var batch = new PayoutBatch
        {
            BatchNumber = await repository.GenerateBatchNumberAsync(now, cancellationToken),
            Currency = currency,
            Status = PayoutBatchStatuses.Draft,
            TotalAmount = wallets.Sum(wallet => wallet.AvailableBalance),
            CreatedByUserId = currentUserContext.UserId,
            CreatedAtUtc = now
        };

        await repository.AddPayoutBatchAsync(batch, cancellationToken);
        foreach (var wallet in wallets)
        {
            await repository.AddPayoutBatchItemAsync(new PayoutBatchItem
            {
                PayoutBatchId = batch.Id,
                TechnicianId = wallet.TechnicianId!.Value,
                TechnicianUserId = wallet.OwnerUserId!.Value,
                WalletId = wallet.Id,
                Amount = wallet.AvailableBalance,
                Currency = currency,
                Status = PayoutBatchItemStatuses.Pending,
                CreatedAtUtc = now
            }, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return await GetPayoutBatchAsync(batch.Id, cancellationToken);
    }

    public async Task<Result<IReadOnlyCollection<PayoutBatchResponse>>> GetPayoutBatchesAsync(CancellationToken cancellationToken = default)
    {
        return Result.Success<IReadOnlyCollection<PayoutBatchResponse>>(await repository.QueryPayoutBatches().AsNoTracking().OrderByDescending(batch => batch.CreatedAtUtc).Select(batch => MapPayoutBatch(batch, null)).ToArrayAsync(cancellationToken));
    }

    public async Task<Result<PayoutBatchResponse>> GetPayoutBatchAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var batch = await repository.QueryPayoutBatches().AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (batch is null) return Result.Failure<PayoutBatchResponse>("payout_batch_not_found", "Payout batch was not found.");
        var items = await repository.QueryPayoutBatchItems().AsNoTracking().Where(item => item.PayoutBatchId == id).OrderBy(item => item.CreatedAtUtc).Select(item => MapPayoutBatchItem(item)).ToArrayAsync(cancellationToken);
        return Result.Success(MapPayoutBatch(batch, items));
    }

    private IQueryable<WalletEntryResponse> GetEntries(Guid walletId)
    {
        return from entry in repository.QueryLedgerEntries().AsNoTracking()
               join transaction in repository.QueryLedgerTransactions().AsNoTracking() on entry.LedgerTransactionId equals transaction.Id
               where entry.WalletId == walletId
               orderby entry.CreatedAtUtc descending
               select new WalletEntryResponse(entry.Id, transaction.Id, transaction.TransactionNumber, transaction.SourceModule, transaction.SourceEntityId, transaction.TransactionType, entry.EntryDirection, entry.BalanceType, entry.Amount, entry.Currency, entry.Description, entry.CreatedAtUtc);
    }

    private static WalletResponse MapWallet(Wallet wallet) => new(wallet.Id, wallet.OwnerType, wallet.OwnerUserId, wallet.TechnicianId, wallet.AvailableBalance, wallet.PendingBalance, wallet.Currency);
    private static LedgerTransactionResponse MapTransaction(LedgerTransaction transaction) => new(transaction.Id, transaction.TransactionNumber, transaction.SourceModule, transaction.SourceEntityId, transaction.TransactionType, transaction.TotalAmount, transaction.Currency, transaction.IdempotencyKey, transaction.Description, transaction.CreatedAtUtc);
    private static PlatformEarningResponse MapPlatformEarning(PlatformEarning earning) => new(earning.Id, earning.PaymentId, earning.JobId, earning.PriceSnapshotId, earning.CustomerUserId, earning.TechnicianId, earning.TechnicianUserId, earning.BasePrice, earning.CommissionRate, earning.CommissionAmount, earning.VatRate, earning.VatAmount, earning.TotalAmount, earning.DepositAmount, earning.Currency, earning.Status, earning.CreatedAtUtc);
    private static TechnicianEarningResponse MapTechnicianEarning(TechnicianEarning earning) => new(earning.Id, earning.JobId, earning.PaymentId, earning.TechnicianId, earning.TechnicianUserId, earning.Amount, earning.Currency, earning.Status, earning.AvailableAtUtc, earning.CreatedAtUtc, earning.UpdatedAtUtc);
    private static PayoutBatchResponse MapPayoutBatch(PayoutBatch batch, IReadOnlyCollection<PayoutBatchItemResponse>? items) => new(batch.Id, batch.BatchNumber, batch.Status, batch.Currency, batch.TotalAmount, batch.CreatedByUserId, batch.CreatedAtUtc, batch.UpdatedAtUtc, items);
    private static PayoutBatchItemResponse MapPayoutBatchItem(PayoutBatchItem item) => new(item.Id, item.PayoutBatchId, item.TechnicianId, item.TechnicianUserId, item.WalletId, item.Amount, item.Currency, item.Status, item.FailureReason, item.CreatedAtUtc, item.UpdatedAtUtc);
}
