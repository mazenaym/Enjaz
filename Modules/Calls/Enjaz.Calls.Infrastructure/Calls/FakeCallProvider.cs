using Enjaz.Calls.Application.Calls;
using Enjaz.Calls.Domain.Calls;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Calls.Infrastructure.Calls;

public sealed class FakeCallProvider : ICallProvider
{
    public string ProviderName => CallProviders.Fake;

    public Task<Result<CreateMaskedCallResult>> CreateMaskedCallAsync(CreateMaskedCallRequest request, CancellationToken cancellationToken = default)
    {
        var providerCallId = $"fake-{Guid.NewGuid():N}";
        var result = new CreateMaskedCallResult(ProviderName, providerCallId, "+200000MASK", CallStatuses.Ringing);
        return Task.FromResult(Result.Success(result));
    }
}
