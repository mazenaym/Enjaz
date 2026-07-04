using Enjaz.Pricing.Domain.Pricing;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Pricing.Application.Pricing;

public interface IPricingCalculator
{
    Task<Result<PriceCalculationResponse>> CalculateAsync(CalculatePriceRequest request, CancellationToken cancellationToken = default);
}

public interface IPricingAdminService
{
    Task<Result<IReadOnlyCollection<PricingRuleResponse>>> GetPricingRulesAsync(CancellationToken cancellationToken = default);
    Task<Result<PricingRuleResponse>> CreatePricingRuleAsync(PricingRuleRequest request, CancellationToken cancellationToken = default);
    Task<Result<PricingRuleResponse>> UpdatePricingRuleAsync(Guid id, PricingRuleRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetPricingRuleActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<CommissionSettingResponse>>> GetCommissionSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result<CommissionSettingResponse>> CreateCommissionSettingAsync(CommissionSettingRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetCommissionDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeactivateCommissionSettingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<VatSettingResponse>>> GetVatSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result<VatSettingResponse>> CreateVatSettingAsync(VatSettingRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetVatDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeactivateVatSettingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<DepositRuleResponse>>> GetDepositRulesAsync(CancellationToken cancellationToken = default);
    Task<Result<DepositRuleResponse>> CreateDepositRuleAsync(DepositRuleRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetDepositDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeactivateDepositRuleAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IPricingRepository
{
    Task<PricingRule?> GetActivePricingRuleAsync(Guid serviceId, int complexityId, DateTime atUtc, CancellationToken cancellationToken = default);
    Task<CommissionSetting?> GetDefaultCommissionSettingAsync(DateTime atUtc, CancellationToken cancellationToken = default);
    Task<VatSetting?> GetDefaultVatSettingAsync(DateTime atUtc, CancellationToken cancellationToken = default);
    Task<DepositRule?> GetDefaultDepositRuleAsync(CancellationToken cancellationToken = default);
    Task AddPriceSnapshotAsync(PriceSnapshot snapshot, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PricingRule>> GetPricingRulesAsync(CancellationToken cancellationToken = default);
    Task<PricingRule?> GetPricingRuleAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddPricingRuleAsync(PricingRule rule, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CommissionSetting>> GetCommissionSettingsAsync(CancellationToken cancellationToken = default);
    Task<CommissionSetting?> GetCommissionSettingAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddCommissionSettingAsync(CommissionSetting setting, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VatSetting>> GetVatSettingsAsync(CancellationToken cancellationToken = default);
    Task<VatSetting?> GetVatSettingAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddVatSettingAsync(VatSetting setting, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DepositRule>> GetDepositRulesAsync(CancellationToken cancellationToken = default);
    Task<DepositRule?> GetDepositRuleAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddDepositRuleAsync(DepositRule rule, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
