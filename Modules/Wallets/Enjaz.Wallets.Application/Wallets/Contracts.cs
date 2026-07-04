using Enjaz.Jobs.Application.Jobs;
using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Domain.Wallets;

namespace Enjaz.Wallets.Application.Wallets;

public interface IWalletsService
{
    Task<Result<WalletResponse>> GetMyWalletAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<WalletEntryResponse>>> GetMyWalletTransactionsAsync(CancellationToken cancellationToken = default);
    Task<Result<WalletResponse>> GetMyTechnicianWalletAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<TechnicianEarningResponse>>> GetMyTechnicianEarningsAsync(string? status, CancellationToken cancellationToken = default);
}

public interface IAdminWalletsService
{
    Task<Result<IReadOnlyCollection<WalletResponse>>> GetWalletsAsync(WalletQuery query, CancellationToken cancellationToken = default);
    Task<Result<WalletResponse>> GetWalletAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<LedgerTransactionResponse>>> GetLedgerTransactionsAsync(LedgerTransactionQuery query, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PlatformEarningResponse>>> GetPlatformEarningsAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<TechnicianEarningResponse>>> GetTechnicianEarningsAsync(string? status, CancellationToken cancellationToken = default);
    Task<Result<PayoutBatchResponse>> CreatePayoutBatchAsync(CreatePayoutBatchRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PayoutBatchResponse>>> GetPayoutBatchesAsync(CancellationToken cancellationToken = default);
    Task<Result<PayoutBatchResponse>> GetPayoutBatchAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ILedgerService
{
    Task<Result<LedgerTransaction>> PostTransactionAsync(PostLedgerTransactionRequest request, CancellationToken cancellationToken = default);
}

public interface IPaymentLedgerService
{
    Task<Result> RecordPaymentCapturedAsync(PaymentCapturedLedgerRequest request, CancellationToken cancellationToken = default);
}

public interface IRefundLedgerService
{
    Task<Result> RecordRefundRequestedAsync(Guid refundRequestId, Guid paymentId, Guid jobId, decimal amount, string currency, string? reason, CancellationToken cancellationToken = default);
}

public interface IReleaseTechnicianEarningsJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IJobWalletLookupService
{
    Task<JobWalletLookupResult?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}

public interface IWalletsRepository
{
    IQueryable<Wallet> QueryWallets();
    IQueryable<LedgerTransaction> QueryLedgerTransactions();
    IQueryable<LedgerEntry> QueryLedgerEntries();
    IQueryable<PlatformEarning> QueryPlatformEarnings();
    IQueryable<TechnicianEarning> QueryTechnicianEarnings();
    IQueryable<PayoutBatch> QueryPayoutBatches();
    IQueryable<PayoutBatchItem> QueryPayoutBatchItems();
    Task<Wallet?> GetWalletAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Wallet?> GetActiveWalletAsync(string ownerType, Guid? ownerUserId, Guid? technicianId, string currency, CancellationToken cancellationToken = default);
    Task<Wallet> EnsureWalletAsync(string ownerType, Guid? ownerUserId, Guid? technicianId, string currency, DateTime nowUtc, CancellationToken cancellationToken = default);
    Task<LedgerTransaction?> GetLedgerTransactionByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    Task AddLedgerTransactionAsync(LedgerTransaction transaction, CancellationToken cancellationToken = default);
    Task AddLedgerEntryAsync(LedgerEntry entry, CancellationToken cancellationToken = default);
    Task AddPlatformEarningAsync(PlatformEarning earning, CancellationToken cancellationToken = default);
    Task AddTechnicianEarningAsync(TechnicianEarning earning, CancellationToken cancellationToken = default);
    Task AddPayoutBatchAsync(PayoutBatch batch, CancellationToken cancellationToken = default);
    Task AddPayoutBatchItemAsync(PayoutBatchItem item, CancellationToken cancellationToken = default);
    Task<string> GenerateTransactionNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
    Task<string> GenerateBatchNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record PostLedgerTransactionRequest(string SourceModule, Guid SourceEntityId, string TransactionType, string Currency, decimal TotalAmount, string IdempotencyKey, string? Description, IReadOnlyCollection<PostLedgerEntryRequest> Entries, DateTime CreatedAtUtc);
public sealed record PostLedgerEntryRequest(Guid WalletId, string EntryDirection, string BalanceType, decimal Amount, string? Description);

public static class PricingSnapshotLookupResultExtensions
{
    public static decimal TechnicianPayoutAmountOrZero(this PriceSnapshotLookupResult snapshot) => snapshot.TechnicianPayoutAmount;
}
