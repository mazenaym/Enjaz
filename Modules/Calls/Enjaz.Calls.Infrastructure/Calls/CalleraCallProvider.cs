using Enjaz.Calls.Application.Calls;
using Enjaz.SharedKernel.Results;
using Microsoft.Extensions.Options;

namespace Enjaz.Calls.Infrastructure.Calls;

public sealed class CalleraCallProvider(IOptions<CallsOptions> options) : ICallProvider
{
    public string ProviderName => "Callera";

    public Task<Result<CreateMaskedCallResult>> CreateMaskedCallAsync(CreateMaskedCallRequest request, CancellationToken cancellationToken = default)
    {
        var callera = options.Value.Callera;
        if (string.IsNullOrWhiteSpace(callera.BaseUrl) || string.IsNullOrWhiteSpace(callera.ApiKey))
        {
            return Task.FromResult(Result.Failure<CreateMaskedCallResult>("callera_not_configured", "Callera provider is selected but BaseUrl or ApiKey is not configured."));
        }

        return Task.FromResult(Result.Failure<CreateMaskedCallResult>("callera_not_implemented", "Callera provider skeleton is present but real API integration is not enabled."));
    }
}
