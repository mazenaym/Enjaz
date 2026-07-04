using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Application.Wallets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Wallets.Endpoints.Wallets;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/wallets")]
public sealed class AdminWalletsController(IAdminWalletsService walletsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetWallets([FromQuery] string? ownerType, [FromQuery] Guid? ownerUserId, [FromQuery] string? currency, CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetWalletsAsync(new WalletQuery(ownerType, ownerUserId, currency), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWallet(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetWalletAsync(id, cancellationToken));
    }

    [HttpGet("ledger-transactions")]
    public async Task<IActionResult> GetLedgerTransactions([FromQuery] string? sourceModule, [FromQuery] Guid? sourceEntityId, [FromQuery] string? transactionType, [FromQuery] DateTime? fromDateUtc, [FromQuery] DateTime? toDateUtc, CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetLedgerTransactionsAsync(new LedgerTransactionQuery(sourceModule, sourceEntityId, transactionType, fromDateUtc, toDateUtc), cancellationToken));
    }

    [HttpGet("platform-earnings")]
    public async Task<IActionResult> GetPlatformEarnings(CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetPlatformEarningsAsync(cancellationToken));
    }

    [HttpGet("technician-earnings")]
    public async Task<IActionResult> GetTechnicianEarnings([FromQuery] string? status, CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetTechnicianEarningsAsync(status, cancellationToken));
    }

    [HttpPost("payout-batches")]
    public async Task<IActionResult> CreatePayoutBatch(CreatePayoutBatchRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.CreatePayoutBatchAsync(request, cancellationToken));
    }

    [HttpGet("payout-batches")]
    public async Task<IActionResult> GetPayoutBatches(CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetPayoutBatchesAsync(cancellationToken));
    }

    [HttpGet("payout-batches/{id:guid}")]
    public async Task<IActionResult> GetPayoutBatch(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await walletsService.GetPayoutBatchAsync(id, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
