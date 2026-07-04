using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Application.Wallets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Wallets.Endpoints.Wallets;

[ApiController]
[Route("api/v1/wallets")]
public sealed class WalletsController(IWalletsService walletsService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyWallet(CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetMyWalletAsync(cancellationToken));
    }

    [HttpGet("me/transactions")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyTransactions(CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetMyWalletTransactionsAsync(cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
