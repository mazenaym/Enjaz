using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Domain.Wallets;

namespace Enjaz.Wallets.Application.Wallets;

public sealed class RefundLedgerService(IWalletsRepository repository, ILedgerService ledgerService) : IRefundLedgerService
{
    public async Task<Result> RecordRefundRequestedAsync(Guid refundRequestId, Guid paymentId, Guid jobId, decimal amount, string currency, string? reason, CancellationToken cancellationToken = default)
    {
        var idempotencyKey = $"refund:{refundRequestId}:reserved";
        if (await repository.GetLedgerTransactionByIdempotencyKeyAsync(idempotencyKey, cancellationToken) is not null)
        {
            return Result.Success();
        }

        var now = DateTime.UtcNow;
        var platformWallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Platform, null, null, currency, now, cancellationToken);
        var providerWallet = await repository.EnsureWalletAsync(WalletOwnerTypes.ExternalPaymentProvider, null, null, currency, now, cancellationToken);

        var result = await ledgerService.PostTransactionAsync(new PostLedgerTransactionRequest(
            LedgerSourceModules.Refunds,
            refundRequestId,
            LedgerTransactionTypes.RefundReserved,
            currency,
            amount,
            idempotencyKey,
            reason ?? $"Refund reserved for payment {paymentId}",
            new[]
            {
                new PostLedgerEntryRequest(platformWallet.Id, LedgerEntryDirections.Debit, LedgerBalanceTypes.Available, amount, "Refund reserve"),
                new PostLedgerEntryRequest(providerWallet.Id, LedgerEntryDirections.Credit, LedgerBalanceTypes.Available, amount, "Refund reserve clearing")
            },
            now), cancellationToken);

        return result.IsFailure ? Result.Failure(result.ErrorCode!, result.ErrorMessage!) : Result.Success();
    }
}
