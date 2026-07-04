using System.Text.Json;
using Enjaz.AI.Application.AI;
using Enjaz.SharedKernel.Results;

namespace Enjaz.AI.Infrastructure.AI;

public sealed class FakeIssueClassifierProvider : IAiProvider
{
    public string ProviderName => "Fake";

    public string? Model => "fake-rule-based-v1";

    public Task<Result<AiProviderClassificationResult>> ClassifyIssueAsync(ClassifyIssueRequest request, CancellationToken cancellationToken = default)
    {
        var text = request.Description.ToLowerInvariant();
        var (complexityId, complexityName, confidence, action, requiresInspection) =
            ContainsAny(text, ["replace", "replacement", "full", "major", "renovation", "project", "كامل", "تغيير", "استبدال"])
                ? (3, "Complex", 0.82m, "Technician inspection is recommended before pricing.", true)
                : ContainsAny(text, ["small", "minor", "leak", "drip", "simple", "weak", "تنقط", "تسريب", "ضعيفة", "بسيط"])
                    ? (1, "Simple", 0.85m, "Likely a small repair that can be priced directly.", false)
                    : (2, "Standard", 0.78m, "Standard issue; assign a qualified technician.", false);

        var payload = new
        {
            complexityId,
            complexityName,
            confidence,
            suggestedAction = action,
            requiresInspection
        };

        return Task.FromResult(Result.Success(new AiProviderClassificationResult(
            complexityId,
            complexityName,
            confidence,
            action,
            requiresInspection,
            JsonSerializer.Serialize(payload),
            ProviderName,
            Model)));
    }

    private static bool ContainsAny(string text, IReadOnlyCollection<string> terms)
    {
        return terms.Any(term => text.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}
