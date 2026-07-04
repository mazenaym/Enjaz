using System.Text.Json;
using Enjaz.AI.Domain.AI;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;

namespace Enjaz.AI.Application.AI;

public sealed class IssueClassifier(
    IAiProvider aiProvider,
    IAiRepository repository,
    ICurrentUserContext currentUserContext) : IIssueClassifier
{
    private const string Feature = "IssueClassification";

    public async Task<Result<ClassifyIssueResponse>> ClassifyAsync(ClassifyIssueRequest request, CancellationToken cancellationToken = default)
    {
        var userId = currentUserContext.IsAuthenticated && currentUserContext.UserId != Guid.Empty
            ? currentUserContext.UserId
            : (Guid?)null;

        var providerResult = await aiProvider.ClassifyIssueAsync(request, cancellationToken);
        var aiRequest = new AiRequest
        {
            UserId = userId,
            Feature = Feature,
            Provider = aiProvider.ProviderName,
            Model = aiProvider.Model,
            InputText = request.Description,
            InputJson = JsonSerializer.Serialize(request),
            RawResponseJson = providerResult.IsSuccess ? providerResult.Value!.RawResponseJson : null,
            Success = providerResult.IsSuccess,
            ErrorMessage = providerResult.IsFailure ? providerResult.ErrorMessage : null,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddRequestAsync(aiRequest, cancellationToken);

        if (providerResult.IsFailure)
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Failure<ClassifyIssueResponse>(providerResult.ErrorCode!, providerResult.ErrorMessage!);
        }

        var classificationResult = providerResult.Value!;
        var classification = new AiClassification
        {
            AiRequestId = aiRequest.Id,
            UserId = userId,
            ServiceCategoryId = request.ServiceCategoryId,
            ServiceId = request.ServiceId,
            CustomerDescription = request.Description.Trim(),
            ComplexityId = classificationResult.ComplexityId,
            ComplexityName = classificationResult.ComplexityName,
            Confidence = classificationResult.Confidence,
            SuggestedAction = classificationResult.SuggestedAction,
            RequiresInspection = classificationResult.RequiresInspection,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddClassificationAsync(classification, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(new ClassifyIssueResponse(
            classification.Id,
            classification.ComplexityId,
            classification.ComplexityName,
            classification.Confidence,
            classification.SuggestedAction,
            classification.RequiresInspection));
    }
}
