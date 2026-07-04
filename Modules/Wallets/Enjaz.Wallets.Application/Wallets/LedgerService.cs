using Enjaz.SharedKernel.Results;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Enjaz.Wallets.Domain.Wallets;

namespace Enjaz.Wallets.Application.Wallets;

public sealed class LedgerService(IWalletsRepository repository, INotificationService notificationService) : ILedgerService
{
    public async Task<Result<LedgerTransaction>> PostTransactionAsync(PostLedgerTransactionRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TotalAmount <= 0)
        {
            return Result.Failure<LedgerTransaction>("invalid_ledger_amount", "Ledger transaction total amount must be greater than zero.");
        }

        if (request.Entries.Count == 0 || request.Entries.Any(entry => entry.Amount <= 0))
        {
            return Result.Failure<LedgerTransaction>("invalid_ledger_entries", "Ledger entries must contain positive amounts.");
        }

        return await repository.ExecuteInTransactionAsync(async ct =>
        {
            var existing = await repository.GetLedgerTransactionByIdempotencyKeyAsync(request.IdempotencyKey, ct);
            if (existing is not null)
            {
                return Result.Success(existing);
            }

            var roundedEntries = request.Entries
                .Select(entry => entry with { Amount = RoundMoney(entry.Amount) })
                .Where(entry => entry.Amount > 0)
                .ToArray();

            if (roundedEntries.Length == 0)
            {
                return Result.Failure<LedgerTransaction>("invalid_ledger_entries", "Ledger entries must contain positive amounts.");
            }

            var credits = roundedEntries.Where(entry => entry.EntryDirection == LedgerEntryDirections.Credit).Sum(entry => entry.Amount);
            var debits = roundedEntries.Where(entry => entry.EntryDirection == LedgerEntryDirections.Debit).Sum(entry => entry.Amount);
            if (credits != debits)
            {
                return Result.Failure<LedgerTransaction>("ledger_not_balanced", "Ledger transaction debits and credits must balance.");
            }

            var now = request.CreatedAtUtc.Kind == DateTimeKind.Utc ? request.CreatedAtUtc : DateTime.UtcNow;
            var transaction = new LedgerTransaction
            {
                TransactionNumber = await repository.GenerateTransactionNumberAsync(now, ct),
                SourceModule = request.SourceModule,
                SourceEntityId = request.SourceEntityId,
                TransactionType = request.TransactionType,
                Currency = NormalizeCurrency(request.Currency),
                TotalAmount = RoundMoney(request.TotalAmount),
                IdempotencyKey = request.IdempotencyKey,
                Description = request.Description,
                CreatedAtUtc = now
            };

            await repository.AddLedgerTransactionAsync(transaction, ct);

            var touchedWallets = new List<(Wallet Wallet, PostLedgerEntryRequest Entry)>();
            foreach (var entryRequest in roundedEntries)
            {
                var wallet = await repository.GetWalletAsync(entryRequest.WalletId, ct);
                if (wallet is null)
                {
                    return Result.Failure<LedgerTransaction>("wallet_not_found", "Ledger entry wallet was not found.");
                }

                ApplyEntry(wallet, entryRequest);
                touchedWallets.Add((wallet, entryRequest));
                await repository.AddLedgerEntryAsync(new LedgerEntry
                {
                    LedgerTransactionId = transaction.Id,
                    WalletId = wallet.Id,
                    EntryDirection = entryRequest.EntryDirection,
                    BalanceType = entryRequest.BalanceType,
                    Amount = entryRequest.Amount,
                    Currency = transaction.Currency,
                    Description = entryRequest.Description,
                    CreatedAtUtc = now
                }, ct);
            }

            await repository.SaveChangesAsync(ct);
            foreach (var item in touchedWallets.Where(item => item.Wallet.OwnerUserId.HasValue && item.Wallet.OwnerType != WalletOwnerTypes.ExternalPaymentProvider))
            {
                await NotifyWalletAsync(item.Wallet, item.Entry, transaction, ct);
            }

            return Result.Success(transaction);
        }, cancellationToken);
    }

    private async Task NotifyWalletAsync(Wallet wallet, PostLedgerEntryRequest entry, LedgerTransaction transaction, CancellationToken cancellationToken)
    {
        try
        {
            var type = transaction.TransactionType == LedgerTransactionTypes.TechnicianEarningReleased
                ? NotificationTypes.TechnicianEarningAvailable
                : wallet.OwnerType == WalletOwnerTypes.Technician && entry.BalanceType == LedgerBalanceTypes.Pending && entry.EntryDirection == LedgerEntryDirections.Credit
                    ? NotificationTypes.TechnicianEarningPending
                    : NotificationTypes.WalletUpdated;

            await notificationService.SendAsync(new SendNotificationRequest(
                wallet.OwnerUserId!.Value,
                type,
                "Wallet updated",
                "Your wallet was updated.",
                new Dictionary<string, string?>
                {
                    ["walletId"] = wallet.Id.ToString(),
                    ["transactionId"] = transaction.Id.ToString(),
                    ["transactionNumber"] = transaction.TransactionNumber,
                    ["amount"] = entry.Amount.ToString("0.00"),
                    ["currency"] = transaction.Currency
                },
                [NotificationChannels.InApp]), cancellationToken);
        }
        catch
        {
            // Wallet ledger posting must stay independent from notification delivery.
        }
    }

    private static void ApplyEntry(Wallet wallet, PostLedgerEntryRequest entry)
    {
        if (entry.EntryDirection == LedgerEntryDirections.Credit)
        {
            if (entry.BalanceType == LedgerBalanceTypes.Available) wallet.AvailableBalance = RoundMoney(wallet.AvailableBalance + entry.Amount);
            if (entry.BalanceType == LedgerBalanceTypes.Pending) wallet.PendingBalance = RoundMoney(wallet.PendingBalance + entry.Amount);
            wallet.TotalCredited = RoundMoney(wallet.TotalCredited + entry.Amount);
        }
        else if (entry.EntryDirection == LedgerEntryDirections.Debit)
        {
            if (entry.BalanceType == LedgerBalanceTypes.Available) wallet.AvailableBalance = RoundMoney(wallet.AvailableBalance - entry.Amount);
            if (entry.BalanceType == LedgerBalanceTypes.Pending) wallet.PendingBalance = RoundMoney(wallet.PendingBalance - entry.Amount);
            wallet.TotalDebited = RoundMoney(wallet.TotalDebited + entry.Amount);
        }
        else
        {
            throw new InvalidOperationException("Unsupported ledger entry direction.");
        }

        if (wallet.OwnerType != WalletOwnerTypes.ExternalPaymentProvider && (wallet.AvailableBalance < 0 || wallet.PendingBalance < 0))
        {
            throw new InvalidOperationException("Wallet balance cannot become negative.");
        }

        wallet.UpdatedAtUtc = DateTime.UtcNow;
    }

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    private static string NormalizeCurrency(string currency) => string.IsNullOrWhiteSpace(currency) ? "EGP" : currency.Trim().ToUpperInvariant();
}
