using FluentValidation;

namespace Enjaz.Pricing.Application.Pricing;

public sealed class CalculatePriceRequestValidator : AbstractValidator<CalculatePriceRequest>
{
    public CalculatePriceRequestValidator()
    {
        RuleFor(request => request.ServiceCategoryId).NotEmpty();
        RuleFor(request => request.ServiceId).NotEmpty();
        RuleFor(request => request.ComplexityId).InclusiveBetween(1, 3);
    }
}

public sealed class PricingRuleRequestValidator : AbstractValidator<PricingRuleRequest>
{
    public PricingRuleRequestValidator()
    {
        RuleFor(request => request.ServiceCategoryId).NotEmpty();
        RuleFor(request => request.ServiceId).NotEmpty();
        RuleFor(request => request.ComplexityId).InclusiveBetween(1, 3);
        RuleFor(request => request.BasePrice).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Currency).NotEmpty().MaximumLength(3);
        RuleFor(request => request.EffectiveFromUtc).NotEmpty();
    }
}

public sealed class CommissionSettingRequestValidator : AbstractValidator<CommissionSettingRequest>
{
    public CommissionSettingRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(200);
        RuleFor(request => request.CommissionRate).InclusiveBetween(0m, 1m);
        RuleFor(request => request.EffectiveFromUtc).NotEmpty();
    }
}

public sealed class VatSettingRequestValidator : AbstractValidator<VatSettingRequest>
{
    public VatSettingRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(200);
        RuleFor(request => request.VatRate).InclusiveBetween(0m, 1m);
        RuleFor(request => request.AppliesOn).Equal("Commission");
        RuleFor(request => request.EffectiveFromUtc).NotEmpty();
    }
}

public sealed class DepositRuleRequestValidator : AbstractValidator<DepositRuleRequest>
{
    public DepositRuleRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(200);
        RuleFor(request => request.DepositType).Must(type => type is "Fixed" or "Percentage");
        RuleFor(request => request.DepositValue).GreaterThanOrEqualTo(0);
        RuleFor(request => request.DepositValue).InclusiveBetween(0m, 1m).When(request => request.DepositType == "Percentage");
        RuleFor(request => request.MinimumDeposit).GreaterThanOrEqualTo(0).When(request => request.MinimumDeposit.HasValue);
        RuleFor(request => request.MaximumDeposit).GreaterThanOrEqualTo(0).When(request => request.MaximumDeposit.HasValue);
    }
}
