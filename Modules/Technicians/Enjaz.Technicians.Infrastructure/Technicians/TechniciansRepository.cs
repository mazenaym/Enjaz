using Enjaz.Technicians.Application.Technicians;
using Enjaz.Technicians.Domain.Technicians;
using Enjaz.Technicians.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Technicians.Infrastructure.Technicians;

public sealed class TechniciansRepository(TechniciansDbContext dbContext) : ITechniciansRepository
{
    public async Task<TechnicianProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles.FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<TechnicianProfile?> GetWithDetailsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await WithDetails().FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<TechnicianProfile?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await WithDetails().FirstOrDefaultAsync(profile => profile.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TechnicianProfile>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await WithDetails()
            .Where(profile => profile.Status == TechnicianStatuses.Pending)
            .OrderBy(profile => profile.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<TechnicianDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianDocuments.FirstOrDefaultAsync(document => document.Id == documentId, cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles.AnyAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task AddProfileAsync(TechnicianProfile profile, CancellationToken cancellationToken = default)
    {
        await dbContext.TechnicianProfiles.AddAsync(profile, cancellationToken);
    }

    public async Task AddDocumentAsync(TechnicianDocument document, CancellationToken cancellationToken = default)
    {
        await dbContext.TechnicianDocuments.AddAsync(document, cancellationToken);
    }

    public async Task AddAvailabilityHistoryAsync(TechnicianAvailabilityHistory history, CancellationToken cancellationToken = default)
    {
        await dbContext.TechnicianAvailabilityHistory.AddAsync(history, cancellationToken);
    }

    public void RemoveSkills(IReadOnlyCollection<TechnicianSkill> skills)
    {
        dbContext.TechnicianSkills.RemoveRange(skills);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TechnicianProfile> WithDetails()
    {
        return dbContext.TechnicianProfiles
            .Include(profile => profile.Documents)
            .Include(profile => profile.Skills)
            .Include(profile => profile.AvailabilityHistory);
    }
}
