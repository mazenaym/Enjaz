using Enjaz.Payments.Application.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Payments.Endpoints.Payments;

[ApiController]
[AllowAnonymous]
[Route("api/v1/payments/webhooks")]
public sealed class PaymentWebhooksController(IPaymentsService paymentsService) : ControllerBase
{
    [HttpPost("paymob")]
    public async Task<IActionResult> Paymob(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        var headers = Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString());
        headers.TryGetValue("hmac", out var signature);
        if (string.IsNullOrWhiteSpace(signature)) headers.TryGetValue("x-paymob-signature", out signature);
        await paymentsService.ProcessPaymobWebhookAsync(rawBody, headers, signature, cancellationToken);
        return Ok(new { message = "Webhook received." });
    }
}
