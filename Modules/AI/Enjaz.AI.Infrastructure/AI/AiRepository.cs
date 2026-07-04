using Enjaz.AI.Application.AI;
using Enjaz.AI.Domain.AI;
using Enjaz.AI.Infrastructure.Persistence;

namespace Enjaz.AI.Infrastructure.AI;

public sealed class AiRepository(AiDbContext dbContext) : IAiRepository
{
    public async Task AddRequestAsync(AiRequest request, CancellationToken cancellationToken = default)
    {
        await dbContext.AiRequests.AddAsync(request, cancellationToken);
    }

    public async Task AddClassificationAsync(AiClassification classification, CancellationToken cancellationToken = default)
    {
        await dbContext.AiClassifications.AddAsync(classification, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
