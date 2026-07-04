using Enjaz.Jobs.Application.Jobs;
using Enjaz.Pricing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Infrastructure.Jobs;

public sealed class PricingSnapshotLookupService(PricingDbContext pricingDbContext) : IPricingSnapshotLookupService
{
    public async Task<PriceSnapshotLookupResult?> GetPriceSnapshotAsync(Guid priceSnapshotId, CancellationToken cancellationToken = default)
    {
        return await pricingDbContext.PriceSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.Id == priceSnapshotId)
            .Select(snapshot => new PriceSnapshotLookupResult(
                snapshot.Id,
                snapshot.UserId,
                snapshot.ServiceCategoryId,
                snapshot.ServiceId,
                snapshot.ComplexityId,
                snapshot.BasePrice,
                snapshot.CommissionRate,
                snapshot.CommissionAmount,
                snapshot.VatRate,
                snapshot.VatAmount,
                snapshot.TotalAmount,
                snapshot.TechnicianPayoutAmount,
                snapshot.DepositAmount,
                snapshot.Currency,
                snapshot.RequiresInspection,
                snapshot.ExpiresAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
