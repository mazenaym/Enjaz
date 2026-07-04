using Enjaz.AI.Domain.AI;
using Enjaz.SharedKernel.Results;

namespace Enjaz.AI.Application.AI;

public interface IIssueClassifier
{
    Task<Result<ClassifyIssueResponse>> ClassifyAsync(ClassifyIssueRequest request, CancellationToken cancellationToken = default);
}

public interface IAiProvider
{
    string ProviderName { get; }
    string? Model { get; }
    Task<Result<AiProviderClassificationResult>> ClassifyIssueAsync(ClassifyIssueRequest request, CancellationToken cancellationToken = default);
}

public interface IAiRepository
{
    Task AddRequestAsync(AiRequest request, CancellationToken cancellationToken = default);
    Task AddClassificationAsync(AiClassification classification, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
