using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Application.Wallets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Wallets.Endpoints.Wallets;

[ApiController]
[Authorize(Roles = "Technician")]
[Route("api/v1/technicians")]
public sealed class TechnicianWalletController(IWalletsService walletsService) : ControllerBase
{
    [HttpGet("wallet")]
    public async Task<IActionResult> GetWallet(CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetMyTechnicianWalletAsync(cancellationToken));
    }

    [HttpGet("earnings")]
    public async Task<IActionResult> GetEarnings([FromQuery] string? status, CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetMyTechnicianEarningsAsync(status, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
