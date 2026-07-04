using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Enjaz.Payments.Application.Payments;
using Enjaz.Payments.Domain.Payments;
using Enjaz.SharedKernel.Results;
using Microsoft.Extensions.Configuration;

namespace Enjaz.Payments.Infrastructure.Payments;

public sealed class PaymobPaymentProvider(IConfiguration configuration) : IPaymentProvider
{
    public string ProviderName => PaymentProviders.Paymob;

    public Task<Result<CreateCheckoutSessionProviderResponse>> CreateCheckoutSessionAsync(CreateCheckoutSessionProviderRequest request, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Payments:Paymob:ApiKey"];
        var iframeId = configuration["Payments:Paymob:IframeId"];
        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(iframeId))
        {
            return Task.FromResult(Result.Failure<CreateCheckoutSessionProviderResponse>("paymob_not_configured", "Paymob configuration is incomplete."));
        }

        return Task.FromResult(Result.Failure<CreateCheckoutSessionProviderResponse>("paymob_not_implemented", "Real Paymob checkout calls are not implemented in Sprint 6."));
    }

    public Task<Result> VerifyWebhookAsync(PaymentWebhookVerificationRequest request, CancellationToken cancellationToken = default)
    {
        var secret = configuration["Payments:Paymob:HmacSecret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            return Task.FromResult(Result.Success());
        }

        if (string.IsNullOrWhiteSpace(request.Signature))
        {
            return Task.FromResult(Result.Failure("missing_signature", "Webhook signature is missing."));
        }

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
        var computed = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(request.RawBody))).ToLowerInvariant();
        return Task.FromResult(string.Equals(computed, request.Signature, StringComparison.OrdinalIgnoreCase)
            ? Result.Success()
            : Result.Failure("invalid_signature", "Webhook signature is invalid."));
    }

    public Task<Result<ParsedPaymentWebhook>> ParseWebhookAsync(PaymentWebhookParseRequest request, CancellationToken cancellationToken = default)
    {
        using var document = JsonDocument.Parse(request.RawBody);
        var root = document.RootElement;
        var obj = root.TryGetProperty("obj", out var objElement) ? objElement : root;
        var success = TryGetBool(obj, "success") ?? TryGetString(obj, "success") == "true";
        var status = success ? "succeeded" : "failed";
        var transactionId = TryGetString(obj, "id") ?? TryGetString(obj, "transaction_id");
        var orderId = TryGetString(obj, "order") ?? TryGetNestedString(obj, "order", "id") ?? TryGetString(obj, "merchant_order_id");
        var amountCents = TryGetDecimal(obj, "amount_cents");
        var amount = amountCents.HasValue ? Math.Round(amountCents.Value / 100m, 2) : TryGetDecimal(obj, "amount");
        var currency = TryGetString(obj, "currency");
        var parsed = new ParsedPaymentWebhook(status == "succeeded" ? PaymentTransactionTypes.PaymentSucceeded : PaymentTransactionTypes.PaymentFailed, transactionId, orderId, status, amount, currency, request.RawBody);
        return Task.FromResult(Result.Success(parsed));
    }

    private static string? TryGetString(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) ? value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString() : null;
    }

    private static string? TryGetNestedString(JsonElement element, string parent, string child)
    {
        return element.TryGetProperty(parent, out var parentElement) && parentElement.ValueKind == JsonValueKind.Object ? TryGetString(parentElement, child) : null;
    }

    private static bool? TryGetBool(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) && value.ValueKind is JsonValueKind.True or JsonValueKind.False ? value.GetBoolean() : null;
    }

    private static decimal? TryGetDecimal(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) && value.TryGetDecimal(out var parsed) ? parsed : null;
    }
}
