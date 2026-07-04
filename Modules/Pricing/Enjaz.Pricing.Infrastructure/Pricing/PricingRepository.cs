using Enjaz.Pricing.Application.Pricing;
using Enjaz.Pricing.Domain.Pricing;
using Enjaz.Pricing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Pricing.Infrastructure.Pricing;

public sealed class PricingRepository(PricingDbContext dbContext) : IPricingRepository
{
    public async Task<PricingRule?> GetActivePricingRuleAsync(Guid serviceId, int complexityId, DateTime atUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.PricingRules
            .Where(rule => rule.ServiceId == serviceId && rule.ComplexityId == complexityId && rule.IsActive && rule.EffectiveFromUtc <= atUtc && (rule.EffectiveToUtc == null || rule.EffectiveToUtc > atUtc))
            .OrderByDescending(rule => rule.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CommissionSetting?> GetDefaultCommissionSettingAsync(DateTime atUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.CommissionSettings
            .Where(setting => setting.IsDefault && setting.IsActive && setting.EffectiveFromUtc <= atUtc && (setting.EffectiveToUtc == null || setting.EffectiveToUtc > atUtc))
            .OrderByDescending(setting => setting.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<VatSetting?> GetDefaultVatSettingAsync(DateTime atUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.VatSettings
            .Where(setting => setting.IsDefault && setting.IsActive && setting.EffectiveFromUtc <= atUtc && (setting.EffectiveToUtc == null || setting.EffectiveToUtc > atUtc))
            .OrderByDescending(setting => setting.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DepositRule?> GetDefaultDepositRuleAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DepositRules.FirstOrDefaultAsync(rule => rule.IsDefault && rule.IsActive, cancellationToken);
    }

    public async Task AddPriceSnapshotAsync(PriceSnapshot snapshot, CancellationToken cancellationToken = default) => await dbContext.PriceSnapshots.AddAsync(snapshot, cancellationToken);
    public async Task<IReadOnlyCollection<PricingRule>> GetPricingRulesAsync(CancellationToken cancellationToken = default) => await dbContext.PricingRules.AsNoTracking().OrderBy(rule => rule.ServiceId).ThenBy(rule => rule.ComplexityId).ToArrayAsync(cancellationToken);
    public async Task<PricingRule?> GetPricingRuleAsync(Guid id, CancellationToken cancellationToken = default) => await dbContext.PricingRules.FirstOrDefaultAsync(rule => rule.Id == id, cancellationToken);
    public async Task AddPricingRuleAsync(PricingRule rule, CancellationToken cancellationToken = default) => await dbContext.PricingRules.AddAsync(rule, cancellationToken);
    public async Task<IReadOnlyCollection<CommissionSetting>> GetCommissionSettingsAsync(CancellationToken cancellationToken = default) => await dbContext.CommissionSettings.OrderByDescending(setting => setting.CreatedAtUtc).ToArrayAsync(cancellationToken);
    public async Task<CommissionSetting?> GetCommissionSettingAsync(Guid id, CancellationToken cancellationToken = default) => await dbContext.CommissionSettings.FirstOrDefaultAsync(setting => setting.Id == id, cancellationToken);
    public async Task AddCommissionSettingAsync(CommissionSetting setting, CancellationToken cancellationToken = default) => await dbContext.CommissionSettings.AddAsync(setting, cancellationToken);
    public async Task<IReadOnlyCollection<VatSetting>> GetVatSettingsAsync(CancellationToken cancellationToken = default) => await dbContext.VatSettings.OrderByDescending(setting => setting.CreatedAtUtc).ToArrayAsync(cancellationToken);
    public async Task<VatSetting?> GetVatSettingAsync(Guid id, CancellationToken cancellationToken = default) => await dbContext.VatSettings.FirstOrDefaultAsync(setting => setting.Id == id, cancellationToken);
    public async Task AddVatSettingAsync(VatSetting setting, CancellationToken cancellationToken = default) => await dbContext.VatSettings.AddAsync(setting, cancellationToken);
    public async Task<IReadOnlyCollection<DepositRule>> GetDepositRulesAsync(CancellationToken cancellationToken = default) => await dbContext.DepositRules.OrderByDescending(rule => rule.CreatedAtUtc).ToArrayAsync(cancellationToken);
    public async Task<DepositRule?> GetDepositRuleAsync(Guid id, CancellationToken cancellationToken = default) => await dbContext.DepositRules.FirstOrDefaultAsync(rule => rule.Id == id, cancellationToken);
    public async Task AddDepositRuleAsync(DepositRule rule, CancellationToken cancellationToken = default) => await dbContext.DepositRules.AddAsync(rule, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await dbContext.SaveChangesAsync(cancellationToken);
}
