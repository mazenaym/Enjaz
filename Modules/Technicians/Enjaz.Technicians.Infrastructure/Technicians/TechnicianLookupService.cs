using Enjaz.Maps.Application.Maps;
using Enjaz.Technicians.Domain.Technicians;
using Enjaz.Technicians.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Technicians.Infrastructure.Technicians;

public sealed class TechnicianLookupService(TechniciansDbContext dbContext) : ITechnicianLookupService
{
    public async Task<TechnicianLookupResult?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles
            .AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .Select(profile => new TechnicianLookupResult(
                profile.Id,
                profile.UserId,
                profile.Status,
                profile.AvailabilityStatus))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsApprovedAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles
            .AsNoTracking()
            .AnyAsync(profile => profile.Id == technicianId && profile.Status == TechnicianStatuses.Approved, cancellationToken);
    }

    public async Task<bool> IsOnlineAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles
            .AsNoTracking()
            .AnyAsync(profile => profile.Id == technicianId && profile.AvailabilityStatus == TechnicianAvailabilityStatuses.Online, cancellationToken);
    }

    public async Task<bool> HasSkillAsync(Guid technicianId, Guid serviceId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianSkills
            .AsNoTracking()
            .AnyAsync(skill => skill.TechnicianId == technicianId && skill.ServiceId == serviceId, cancellationToken);
    }
}
