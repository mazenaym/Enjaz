using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Wallets.Application.Wallets;

public sealed class ReleaseTechnicianEarningsJob(IWalletsRepository repository, ILedgerService ledgerService) : IReleaseTechnicianEarningsJob
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var earnings = await repository.QueryTechnicianEarnings()
            .Where(earning => earning.Status == TechnicianEarningStatuses.Pending && earning.AvailableAtUtc.HasValue && earning.AvailableAtUtc <= now && earning.TechnicianId.HasValue && earning.TechnicianUserId.HasValue)
            .Take(100)
            .ToArrayAsync(cancellationToken);

        foreach (var earning in earnings)
        {
            var idempotencyKey = $"technician-earning:{earning.Id}:released";
            if (await repository.GetLedgerTransactionByIdempotencyKeyAsync(idempotencyKey, cancellationToken) is not null)
            {
                earning.Status = TechnicianEarningStatuses.Available;
                earning.UpdatedAtUtc = now;
                continue;
            }

            var wallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Technician, earning.TechnicianUserId, earning.TechnicianId, earning.Currency, now, cancellationToken);
            var result = await ledgerService.PostTransactionAsync(new PostLedgerTransactionRequest(
                LedgerSourceModules.Payouts,
                earning.Id,
                LedgerTransactionTypes.TechnicianEarningReleased,
                earning.Currency,
                earning.Amount,
                idempotencyKey,
                "Technician earning released from pending to available",
                new[]
                {
                    new PostLedgerEntryRequest(wallet.Id, LedgerEntryDirections.Debit, LedgerBalanceTypes.Pending, earning.Amount, "Release pending earning"),
                    new PostLedgerEntryRequest(wallet.Id, LedgerEntryDirections.Credit, LedgerBalanceTypes.Available, earning.Amount, "Release available earning")
                },
                now), cancellationToken);

            if (result.IsSuccess)
            {
                earning.Status = TechnicianEarningStatuses.Available;
                earning.UpdatedAtUtc = now;
                await repository.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
