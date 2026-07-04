using Enjaz.Jobs.Application.Jobs;
using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Domain.Wallets;

namespace Enjaz.Wallets.Application.Wallets;

public sealed class PaymentLedgerService(
    IWalletsRepository repository,
    ILedgerService ledgerService,
    IPricingSnapshotLookupService pricingSnapshotLookupService,
    IJobWalletLookupService jobWalletLookupService)
    : IPaymentLedgerService
{
    public async Task<Result> RecordPaymentCapturedAsync(PaymentCapturedLedgerRequest request, CancellationToken cancellationToken = default)
    {
        var idempotencyKey = $"payment:{request.PaymentId}:captured";
        if (await repository.GetLedgerTransactionByIdempotencyKeyAsync(idempotencyKey, cancellationToken) is not null)
        {
            return Result.Success();
        }

        var snapshot = await pricingSnapshotLookupService.GetPriceSnapshotAsync(request.PriceSnapshotId, cancellationToken);
        if (snapshot is null)
        {
            return Result.Failure("price_snapshot_not_found", "Price snapshot was not found for wallet ledger posting.");
        }

        var job = await jobWalletLookupService.GetJobAsync(request.JobId, cancellationToken);
        if (job is null)
        {
            return Result.Failure("job_not_found", "Job was not found for wallet ledger posting.");
        }

        var currency = NormalizeCurrency(request.Currency);
        var totalAmount = RoundMoney(snapshot.TotalAmount);
        var amountPaid = RoundMoney(request.AmountPaid);
        if (totalAmount <= 0 || amountPaid <= 0)
        {
            return Result.Failure("invalid_payment_amount", "Payment ledger amount must be greater than zero.");
        }

        var paidRatio = Math.Min(1m, amountPaid / totalAmount);
        var recognizedCommission = RoundMoney(snapshot.CommissionAmount * paidRatio);
        var recognizedVat = RoundMoney(snapshot.VatAmount * paidRatio);
        var recognizedTechnicianAmount = RoundMoney(snapshot.TechnicianPayoutAmount * paidRatio);
        var allocated = recognizedCommission + recognizedVat + recognizedTechnicianAmount;
        var roundingDelta = amountPaid - allocated;
        recognizedTechnicianAmount = RoundMoney(recognizedTechnicianAmount + roundingDelta);
        if (recognizedTechnicianAmount < 0)
        {
            recognizedCommission = RoundMoney(recognizedCommission + recognizedTechnicianAmount);
            recognizedTechnicianAmount = 0;
        }

        var now = DateTime.UtcNow;
        var customerWallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Customer, request.CustomerUserId, null, currency, now, cancellationToken);
        var providerWallet = await repository.EnsureWalletAsync(WalletOwnerTypes.ExternalPaymentProvider, null, null, currency, now, cancellationToken);
        var platformWallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Platform, null, null, currency, now, cancellationToken);
        var taxWallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Tax, null, null, currency, now, cancellationToken);
        Wallet? technicianWallet = null;
        if (job.AssignedTechnicianId.HasValue && job.AssignedTechnicianUserId.HasValue)
        {
            technicianWallet = await repository.EnsureWalletAsync(WalletOwnerTypes.Technician, job.AssignedTechnicianUserId, job.AssignedTechnicianId, currency, now, cancellationToken);
        }

        await repository.AddPlatformEarningAsync(new PlatformEarning
        {
            PaymentId = request.PaymentId,
            JobId = request.JobId,
            PriceSnapshotId = request.PriceSnapshotId,
            CustomerUserId = request.CustomerUserId,
            TechnicianId = job.AssignedTechnicianId,
            TechnicianUserId = job.AssignedTechnicianUserId,
            BasePrice = snapshot.BasePrice,
            CommissionRate = snapshot.CommissionRate,
            CommissionAmount = recognizedCommission,
            VatRate = snapshot.VatRate,
            VatAmount = recognizedVat,
            TotalAmount = amountPaid,
            DepositAmount = RoundMoney(snapshot.DepositAmount),
            Currency = currency,
            Status = PlatformEarningStatuses.Recorded,
            CreatedAtUtc = now
        }, cancellationToken);

        if (recognizedTechnicianAmount > 0)
        {
            await repository.AddTechnicianEarningAsync(new TechnicianEarning
            {
                JobId = request.JobId,
                PaymentId = request.PaymentId,
                TechnicianId = job.AssignedTechnicianId,
                TechnicianUserId = job.AssignedTechnicianUserId,
                Amount = recognizedTechnicianAmount,
                Currency = currency,
                Status = TechnicianEarningStatuses.Pending,
                CreatedAtUtc = now
            }, cancellationToken);
        }

        var entries = new List<PostLedgerEntryRequest>
        {
            new(providerWallet.Id, LedgerEntryDirections.Debit, LedgerBalanceTypes.Available, amountPaid, $"External payment captured for job {job.JobNumber}"),
            new(customerWallet.Id, LedgerEntryDirections.Debit, LedgerBalanceTypes.Available, amountPaid, $"Customer paid externally for job {job.JobNumber}")
        };

        if (recognizedCommission > 0) entries.Add(new(platformWallet.Id, LedgerEntryDirections.Credit, LedgerBalanceTypes.Available, recognizedCommission, "Platform commission earned"));
        if (recognizedVat > 0) entries.Add(new(taxWallet.Id, LedgerEntryDirections.Credit, LedgerBalanceTypes.Available, recognizedVat, "VAT liability recorded"));
        if (recognizedTechnicianAmount > 0 && technicianWallet is not null) entries.Add(new(technicianWallet.Id, LedgerEntryDirections.Credit, LedgerBalanceTypes.Pending, recognizedTechnicianAmount, "Technician earning pending"));
        if (recognizedTechnicianAmount > 0 && technicianWallet is null) entries.Add(new(platformWallet.Id, LedgerEntryDirections.Credit, LedgerBalanceTypes.Pending, recognizedTechnicianAmount, "Unassigned technician earning pending"));

        var credits = entries.Where(entry => entry.EntryDirection == LedgerEntryDirections.Credit).Sum(entry => entry.Amount);
        var customerDebit = entries.First(entry => entry.WalletId == customerWallet.Id);
        entries[1] = customerDebit with { Amount = RoundMoney(credits - amountPaid) };
        if (entries[1].Amount <= 0) entries.RemoveAt(1);

        var result = await ledgerService.PostTransactionAsync(new PostLedgerTransactionRequest(
            LedgerSourceModules.Payments,
            request.PaymentId,
            LedgerTransactionTypes.PaymentCaptured,
            currency,
            amountPaid,
            idempotencyKey,
            $"Payment captured for job {job.JobNumber}",
            entries,
            request.CreatedAtUtc), cancellationToken);

        return result.IsFailure ? Result.Failure(result.ErrorCode!, result.ErrorMessage!) : Result.Success();
    }

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    private static string NormalizeCurrency(string currency) => string.IsNullOrWhiteSpace(currency) ? "EGP" : currency.Trim().ToUpperInvariant();
}
