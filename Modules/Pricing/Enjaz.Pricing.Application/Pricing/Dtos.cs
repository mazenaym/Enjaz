namespace Enjaz.Pricing.Application.Pricing;

public sealed record CalculatePriceRequest(Guid ServiceCategoryId, Guid ServiceId, int ComplexityId, Guid? ClassificationId);

public sealed record PriceBreakdownResponse(string Formula, string VatAppliesOn);

public sealed record PriceCalculationResponse(
    Guid PriceSnapshotId,
    bool RequiresInspection,
    string Currency,
    decimal BasePrice,
    decimal CommissionRate,
    decimal CommissionAmount,
    decimal VatRate,
    decimal VatAmount,
    decimal TotalAmount,
    decimal TechnicianPayoutAmount,
    decimal DepositAmount,
    PriceBreakdownResponse Breakdown);

public sealed record PricingRuleRequest(
    Guid ServiceCategoryId,
    Guid ServiceId,
    int ComplexityId,
    decimal BasePrice,
    string Currency,
    bool RequiresInspection,
    DateTime EffectiveFromUtc,
    DateTime? EffectiveToUtc);

public sealed record PricingRuleResponse(Guid Id, Guid ServiceCategoryId, Guid ServiceId, int ComplexityId, decimal BasePrice, string Currency, bool RequiresInspection, bool IsActive, DateTime EffectiveFromUtc, DateTime? EffectiveToUtc);

public sealed record CommissionSettingRequest(string Name, decimal CommissionRate, bool IsDefault, DateTime EffectiveFromUtc, DateTime? EffectiveToUtc);

public sealed record CommissionSettingResponse(Guid Id, string Name, decimal CommissionRate, bool IsDefault, bool IsActive, DateTime EffectiveFromUtc, DateTime? EffectiveToUtc);

public sealed record VatSettingRequest(string Name, decimal VatRate, string AppliesOn, bool IsDefault, DateTime EffectiveFromUtc, DateTime? EffectiveToUtc);

public sealed record VatSettingResponse(Guid Id, string Name, decimal VatRate, string AppliesOn, bool IsDefault, bool IsActive, DateTime EffectiveFromUtc, DateTime? EffectiveToUtc);

public sealed record DepositRuleRequest(string Name, string DepositType, decimal DepositValue, decimal? MinimumDeposit, decimal? MaximumDeposit, bool IsDefault);

public sealed record DepositRuleResponse(Guid Id, string Name, string DepositType, decimal DepositValue, decimal? MinimumDeposit, decimal? MaximumDeposit, bool IsDefault, bool IsActive);
