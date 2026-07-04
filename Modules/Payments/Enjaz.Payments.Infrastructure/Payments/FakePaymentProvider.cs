using System.Text.Json;
using Enjaz.Payments.Application.Payments;
using Enjaz.Payments.Domain.Payments;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Payments.Infrastructure.Payments;

public sealed class FakePaymentProvider : IPaymentProvider
{
    public string ProviderName => PaymentProviders.Fake;

    public Task<Result<CreateCheckoutSessionProviderResponse>> CreateCheckoutSessionAsync(CreateCheckoutSessionProviderRequest request, CancellationToken cancellationToken = default)
    {
        var checkoutUrl = $"http://localhost:5088/api/v1/payments/fake/checkout/{request.PaymentId}";
        var response = new CreateCheckoutSessionProviderResponse(ProviderName, $"fake-order-{request.PaymentId}", $"fake-key-{request.PaymentId}", checkoutUrl, JsonSerializer.Serialize(new { provider = ProviderName, checkoutUrl }));
        return Task.FromResult(Result.Success(response));
    }

    public Task<Result> VerifyWebhookAsync(PaymentWebhookVerificationRequest request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success());

    public Task<Result<ParsedPaymentWebhook>> ParseWebhookAsync(PaymentWebhookParseRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<ParsedPaymentWebhook>("unsupported_fake_webhook", "Fake provider does not parse Paymob webhooks."));
    }
}
