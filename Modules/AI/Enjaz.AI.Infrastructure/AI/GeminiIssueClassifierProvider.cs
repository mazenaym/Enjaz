using Enjaz.AI.Application.AI;
using Enjaz.SharedKernel.Results;
using Microsoft.Extensions.Configuration;

namespace Enjaz.AI.Infrastructure.AI;

public sealed class GeminiIssueClassifierProvider(IConfiguration configuration) : IAiProvider
{
    public string ProviderName => "Gemini";

    public string? Model => configuration["Ai:Gemini:Model"];

    public Task<Result<AiProviderClassificationResult>> ClassifyIssueAsync(ClassifyIssueRequest request, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Ai:Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Task.FromResult(Result.Failure<AiProviderClassificationResult>("ai_provider_not_configured", "Gemini provider is not configured."));
        }

        return Task.FromResult(Result.Failure<AiProviderClassificationResult>("ai_provider_not_implemented", "Gemini provider integration is not implemented yet."));
    }
}
