namespace Enjaz.AI.Application.AI;

public sealed record ClassifyIssueRequest(Guid ServiceCategoryId, Guid ServiceId, string Description);

public sealed record ClassifyIssueResponse(
    Guid ClassificationId,
    int ComplexityId,
    string ComplexityName,
    decimal Confidence,
    string? SuggestedAction,
    bool RequiresInspection);

public sealed record AiProviderClassificationResult(
    int ComplexityId,
    string ComplexityName,
    decimal Confidence,
    string? SuggestedAction,
    bool RequiresInspection,
    string RawResponseJson,
    string Provider,
    string? Model);
