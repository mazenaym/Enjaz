using System.Text.Json;
using Enjaz.Pricing.Domain.Pricing;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Pricing.Application.Pricing;

public sealed class PricingService(
    IPricingRepository repository,
    ICurrentUserContext currentUserContext) : IPricingCalculator, IPricingAdminService
{
    public async Task<Result<PriceCalculationResponse>> CalculateAsync(CalculatePriceRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var rule = await repository.GetActivePricingRuleAsync(request.ServiceId, request.ComplexityId, now, cancellationToken);
        if (rule is null)
        {
            return Result.Failure<PriceCalculationResponse>("pricing_rule_not_found", "Pricing rule was not found.");
        }

        var commission = await repository.GetDefaultCommissionSettingAsync(now, cancellationToken);
        if (commission is null)
        {
            return Result.Failure<PriceCalculationResponse>("commission_setting_not_found", "Default commission setting was not found.");
        }

        var vat = await repository.GetDefaultVatSettingAsync(now, cancellationToken);
        if (vat is null)
        {
            return Result.Failure<PriceCalculationResponse>("vat_setting_not_found", "Default VAT setting was not found.");
        }

        var depositRule = await repository.GetDefaultDepositRuleAsync(cancellationToken);
        if (depositRule is null)
        {
            return Result.Failure<PriceCalculationResponse>("deposit_rule_not_found", "Default deposit rule was not found.");
        }

        var basePrice = rule.RequiresInspection ? 0m : RoundMoney(rule.BasePrice);
        var commissionAmount = rule.RequiresInspection ? 0m : RoundMoney(basePrice * commission.CommissionRate);
        var vatAmount = rule.RequiresInspection ? 0m : RoundMoney(commissionAmount * vat.VatRate);
        var totalAmount = rule.RequiresInspection ? 0m : RoundMoney(basePrice + commissionAmount + vatAmount);
        var technicianPayout = rule.RequiresInspection ? 0m : basePrice;
        var depositAmount = rule.RequiresInspection ? 0m : CalculateDeposit(totalAmount, depositRule);
        var breakdown = new PriceBreakdownResponse("total = base + commission + vat", vat.AppliesOn);

        var snapshot = new PriceSnapshot
        {
            UserId = currentUserContext.IsAuthenticated && currentUserContext.UserId != Guid.Empty ? currentUserContext.UserId : null,
            ServiceCategoryId = request.ServiceCategoryId,
            ServiceId = request.ServiceId,
            ComplexityId = request.ComplexityId,
            PricingRuleId = rule.Id,
            CommissionSettingId = commission.Id,
            VatSettingId = vat.Id,
            DepositRuleId = depositRule.Id,
            BasePrice = basePrice,
            CommissionRate = commission.CommissionRate,
            CommissionAmount = commissionAmount,
            VatRate = vat.VatRate,
            VatAmount = vatAmount,
            TotalAmount = totalAmount,
            TechnicianPayoutAmount = technicianPayout,
            DepositAmount = depositAmount,
            Currency = rule.Currency,
            RequiresInspection = rule.RequiresInspection,
            BreakdownJson = JsonSerializer.Serialize(breakdown),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(30)
        };

        await repository.AddPriceSnapshotAsync(snapshot, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(new PriceCalculationResponse(
            snapshot.Id,
            snapshot.RequiresInspection,
            snapshot.Currency,
            snapshot.BasePrice,
            snapshot.CommissionRate,
            snapshot.CommissionAmount,
            snapshot.VatRate,
            snapshot.VatAmount,
            snapshot.TotalAmount,
            snapshot.TechnicianPayoutAmount,
            snapshot.DepositAmount,
            breakdown));
    }

    public async Task<Result<IReadOnlyCollection<PricingRuleResponse>>> GetPricingRulesAsync(CancellationToken cancellationToken = default)
    {
        var rules = await repository.GetPricingRulesAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<PricingRuleResponse>>(rules.Select(Map).ToArray());
    }

    public async Task<Result<PricingRuleResponse>> CreatePricingRuleAsync(PricingRuleRequest request, CancellationToken cancellationToken = default)
    {
        var rule = new PricingRule
        {
            ServiceCategoryId = request.ServiceCategoryId,
            ServiceId = request.ServiceId,
            ComplexityId = request.ComplexityId,
            BasePrice = request.BasePrice,
            Currency = request.Currency.Trim().ToUpperInvariant(),
            RequiresInspection = request.RequiresInspection,
            IsActive = true,
            EffectiveFromUtc = request.EffectiveFromUtc,
            EffectiveToUtc = request.EffectiveToUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddPricingRuleAsync(rule, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(rule));
    }

    public async Task<Result<PricingRuleResponse>> UpdatePricingRuleAsync(Guid id, PricingRuleRequest request, CancellationToken cancellationToken = default)
    {
        var rule = await repository.GetPricingRuleAsync(id, cancellationToken);
        if (rule is null)
        {
            return Result.Failure<PricingRuleResponse>("pricing_rule_not_found", "Pricing rule was not found.");
        }

        rule.ServiceCategoryId = request.ServiceCategoryId;
        rule.ServiceId = request.ServiceId;
        rule.ComplexityId = request.ComplexityId;
        rule.BasePrice = request.BasePrice;
        rule.Currency = request.Currency.Trim().ToUpperInvariant();
        rule.RequiresInspection = request.RequiresInspection;
        rule.EffectiveFromUtc = request.EffectiveFromUtc;
        rule.EffectiveToUtc = request.EffectiveToUtc;
        rule.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(rule));
    }

    public async Task<Result> SetPricingRuleActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var rule = await repository.GetPricingRuleAsync(id, cancellationToken);
        if (rule is null)
        {
            return Result.Failure("pricing_rule_not_found", "Pricing rule was not found.");
        }

        rule.IsActive = isActive;
        rule.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<CommissionSettingResponse>>> GetCommissionSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await repository.GetCommissionSettingsAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<CommissionSettingResponse>>(settings.Select(Map).ToArray());
    }

    public async Task<Result<CommissionSettingResponse>> CreateCommissionSettingAsync(CommissionSettingRequest request, CancellationToken cancellationToken = default)
    {
        var setting = new CommissionSetting { Name = request.Name.Trim(), CommissionRate = request.CommissionRate, IsDefault = request.IsDefault, IsActive = true, EffectiveFromUtc = request.EffectiveFromUtc, EffectiveToUtc = request.EffectiveToUtc, CreatedAtUtc = DateTime.UtcNow };
        await repository.AddCommissionSettingAsync(setting, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(setting));
    }

    public async Task<Result> SetCommissionDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await SetDefaultAsync(await repository.GetCommissionSettingsAsync(cancellationToken), id, cancellationToken);
    }

    public async Task<Result> DeactivateCommissionSettingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var setting = await repository.GetCommissionSettingAsync(id, cancellationToken);
        if (setting is null) return Result.Failure("commission_setting_not_found", "Commission setting was not found.");
        setting.IsActive = false;
        setting.IsDefault = false;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<VatSettingResponse>>> GetVatSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await repository.GetVatSettingsAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<VatSettingResponse>>(settings.Select(Map).ToArray());
    }

    public async Task<Result<VatSettingResponse>> CreateVatSettingAsync(VatSettingRequest request, CancellationToken cancellationToken = default)
    {
        var setting = new VatSetting { Name = request.Name.Trim(), VatRate = request.VatRate, AppliesOn = request.AppliesOn, IsDefault = request.IsDefault, IsActive = true, EffectiveFromUtc = request.EffectiveFromUtc, EffectiveToUtc = request.EffectiveToUtc, CreatedAtUtc = DateTime.UtcNow };
        await repository.AddVatSettingAsync(setting, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(setting));
    }

    public async Task<Result> SetVatDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await SetDefaultAsync(await repository.GetVatSettingsAsync(cancellationToken), id, cancellationToken);
    }

    public async Task<Result> DeactivateVatSettingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var setting = await repository.GetVatSettingAsync(id, cancellationToken);
        if (setting is null) return Result.Failure("vat_setting_not_found", "VAT setting was not found.");
        setting.IsActive = false;
        setting.IsDefault = false;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<DepositRuleResponse>>> GetDepositRulesAsync(CancellationToken cancellationToken = default)
    {
        var rules = await repository.GetDepositRulesAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<DepositRuleResponse>>(rules.Select(Map).ToArray());
    }

    public async Task<Result<DepositRuleResponse>> CreateDepositRuleAsync(DepositRuleRequest request, CancellationToken cancellationToken = default)
    {
        var rule = new DepositRule { Name = request.Name.Trim(), DepositType = request.DepositType, DepositValue = request.DepositValue, MinimumDeposit = request.MinimumDeposit, MaximumDeposit = request.MaximumDeposit, IsDefault = request.IsDefault, IsActive = true, CreatedAtUtc = DateTime.UtcNow };
        await repository.AddDepositRuleAsync(rule, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(rule));
    }

    public async Task<Result> SetDepositDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await SetDefaultAsync(await repository.GetDepositRulesAsync(cancellationToken), id, cancellationToken);
    }

    public async Task<Result> DeactivateDepositRuleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rule = await repository.GetDepositRuleAsync(id, cancellationToken);
        if (rule is null) return Result.Failure("deposit_rule_not_found", "Deposit rule was not found.");
        rule.IsActive = false;
        rule.IsDefault = false;
        rule.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> SetDefaultAsync<T>(IReadOnlyCollection<T> settings, Guid id, CancellationToken cancellationToken)
    {
        var selected = settings.FirstOrDefault(setting => (Guid)setting!.GetType().GetProperty("Id")!.GetValue(setting)! == id);
        if (selected is null)
        {
            return Result.Failure("setting_not_found", "Setting was not found.");
        }

        foreach (var setting in settings)
        {
            setting!.GetType().GetProperty("IsDefault")!.SetValue(setting, false);
        }

        selected.GetType().GetProperty("IsDefault")!.SetValue(selected, true);
        selected.GetType().GetProperty("IsActive")!.SetValue(selected, true);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static decimal CalculateDeposit(decimal totalAmount, DepositRule rule)
    {
        var deposit = rule.DepositType == "Fixed" ? rule.DepositValue : totalAmount * rule.DepositValue;
        if (rule.MinimumDeposit.HasValue) deposit = Math.Max(deposit, rule.MinimumDeposit.Value);
        if (rule.MaximumDeposit.HasValue) deposit = Math.Min(deposit, rule.MaximumDeposit.Value);
        return RoundMoney(deposit);
    }

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    private static PricingRuleResponse Map(PricingRule rule) => new(rule.Id, rule.ServiceCategoryId, rule.ServiceId, rule.ComplexityId, rule.BasePrice, rule.Currency, rule.RequiresInspection, rule.IsActive, rule.EffectiveFromUtc, rule.EffectiveToUtc);
    private static CommissionSettingResponse Map(CommissionSetting setting) => new(setting.Id, setting.Name, setting.CommissionRate, setting.IsDefault, setting.IsActive, setting.EffectiveFromUtc, setting.EffectiveToUtc);
    private static VatSettingResponse Map(VatSetting setting) => new(setting.Id, setting.Name, setting.VatRate, setting.AppliesOn, setting.IsDefault, setting.IsActive, setting.EffectiveFromUtc, setting.EffectiveToUtc);
    private static DepositRuleResponse Map(DepositRule rule) => new(rule.Id, rule.Name, rule.DepositType, rule.DepositValue, rule.MinimumDeposit, rule.MaximumDeposit, rule.IsDefault, rule.IsActive);
}
