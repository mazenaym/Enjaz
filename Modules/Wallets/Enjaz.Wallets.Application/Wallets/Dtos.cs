namespace Enjaz.Wallets.Application.Wallets;

public sealed record WalletResponse(Guid Id, string OwnerType, Guid? OwnerUserId, Guid? TechnicianId, decimal AvailableBalance, decimal PendingBalance, string Currency);
public sealed record WalletEntryResponse(Guid EntryId, Guid LedgerTransactionId, string TransactionNumber, string SourceModule, Guid SourceEntityId, string TransactionType, string EntryDirection, string BalanceType, decimal Amount, string Currency, string? Description, DateTime CreatedAtUtc);
public sealed record LedgerTransactionResponse(Guid Id, string TransactionNumber, string SourceModule, Guid SourceEntityId, string TransactionType, decimal TotalAmount, string Currency, string IdempotencyKey, string? Description, DateTime CreatedAtUtc);
public sealed record PlatformEarningResponse(Guid Id, Guid PaymentId, Guid JobId, Guid PriceSnapshotId, Guid CustomerUserId, Guid? TechnicianId, Guid? TechnicianUserId, decimal BasePrice, decimal CommissionRate, decimal CommissionAmount, decimal VatRate, decimal VatAmount, decimal TotalAmount, decimal DepositAmount, string Currency, string Status, DateTime CreatedAtUtc);
public sealed record TechnicianEarningResponse(Guid Id, Guid JobId, Guid PaymentId, Guid? TechnicianId, Guid? TechnicianUserId, decimal Amount, string Currency, string Status, DateTime? AvailableAtUtc, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
public sealed record CreatePayoutBatchRequest(IReadOnlyCollection<Guid> TechnicianIds, string? Currency);
public sealed record PayoutBatchResponse(Guid Id, string BatchNumber, string Status, string Currency, decimal TotalAmount, Guid? CreatedByUserId, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, IReadOnlyCollection<PayoutBatchItemResponse>? Items = null);
public sealed record PayoutBatchItemResponse(Guid Id, Guid PayoutBatchId, Guid TechnicianId, Guid TechnicianUserId, Guid WalletId, decimal Amount, string Currency, string Status, string? FailureReason, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
public sealed record LedgerTransactionQuery(string? SourceModule, Guid? SourceEntityId, string? TransactionType, DateTime? FromDateUtc, DateTime? ToDateUtc);
public sealed record WalletQuery(string? OwnerType, Guid? OwnerUserId, string? Currency);

public sealed record PaymentCapturedLedgerRequest(Guid PaymentId, Guid JobId, Guid PriceSnapshotId, Guid CustomerUserId, decimal AmountPaid, string Currency, DateTime CreatedAtUtc);
public sealed record JobWalletLookupResult(Guid JobId, string JobNumber, Guid CustomerUserId, Guid PriceSnapshotId, Guid? AssignedTechnicianId, Guid? AssignedTechnicianUserId, string Status);
