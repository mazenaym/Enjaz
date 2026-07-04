using Enjaz.Pricing.Application.Pricing;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Pricing.Endpoints.Pricing;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/pricing")]
public sealed class AdminPricingController(IPricingAdminService pricingAdminService) : ControllerBase
{
    [HttpGet("rules")]
    public async Task<IActionResult> GetRules(CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.GetPricingRulesAsync(cancellationToken));
    [HttpPost("rules")]
    public async Task<IActionResult> CreateRule(PricingRuleRequest request, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.CreatePricingRuleAsync(request, cancellationToken));
    [HttpPut("rules/{id:guid}")]
    public async Task<IActionResult> UpdateRule(Guid id, PricingRuleRequest request, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.UpdatePricingRuleAsync(id, request, cancellationToken));
    [HttpPost("rules/{id:guid}/activate")]
    public async Task<IActionResult> ActivateRule(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.SetPricingRuleActiveAsync(id, true, cancellationToken));
    [HttpPost("rules/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateRule(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.SetPricingRuleActiveAsync(id, false, cancellationToken));
    [HttpGet("commission-settings")]
    public async Task<IActionResult> GetCommissionSettings(CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.GetCommissionSettingsAsync(cancellationToken));
    [HttpPost("commission-settings")]
    public async Task<IActionResult> CreateCommissionSetting(CommissionSettingRequest request, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.CreateCommissionSettingAsync(request, cancellationToken));
    [HttpPost("commission-settings/{id:guid}/set-default")]
    public async Task<IActionResult> SetCommissionDefault(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.SetCommissionDefaultAsync(id, cancellationToken));
    [HttpPost("commission-settings/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateCommission(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.DeactivateCommissionSettingAsync(id, cancellationToken));
    [HttpGet("vat-settings")]
    public async Task<IActionResult> GetVatSettings(CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.GetVatSettingsAsync(cancellationToken));
    [HttpPost("vat-settings")]
    public async Task<IActionResult> CreateVatSetting(VatSettingRequest request, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.CreateVatSettingAsync(request, cancellationToken));
    [HttpPost("vat-settings/{id:guid}/set-default")]
    public async Task<IActionResult> SetVatDefault(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.SetVatDefaultAsync(id, cancellationToken));
    [HttpPost("vat-settings/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateVat(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.DeactivateVatSettingAsync(id, cancellationToken));
    [HttpGet("deposit-rules")]
    public async Task<IActionResult> GetDepositRules(CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.GetDepositRulesAsync(cancellationToken));
    [HttpPost("deposit-rules")]
    public async Task<IActionResult> CreateDepositRule(DepositRuleRequest request, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.CreateDepositRuleAsync(request, cancellationToken));
    [HttpPost("deposit-rules/{id:guid}/set-default")]
    public async Task<IActionResult> SetDepositDefault(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.SetDepositDefaultAsync(id, cancellationToken));
    [HttpPost("deposit-rules/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateDeposit(Guid id, CancellationToken cancellationToken) => ToActionResult(await pricingAdminService.DeactivateDepositRuleAsync(id, cancellationToken));

    private static IActionResult ToActionResult(Result result) => result.IsSuccess ? new OkObjectResult(new { message = "Success" }) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
