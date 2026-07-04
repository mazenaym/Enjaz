using Enjaz.Maps.Application.Maps;
using Enjaz.Reviews.Application.Reviews;
using Enjaz.Technicians.Domain.Technicians;
using Enjaz.Technicians.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Technicians.Infrastructure.Technicians;

public sealed class TechnicianLookupService(TechniciansDbContext dbContext) : ITechnicianLookupService, IReviewTechnicianLookupService
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

    public async Task<TechnicianLookupResult?> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles
            .AsNoTracking()
            .Where(profile => profile.Id == technicianId)
            .Select(profile => new TechnicianLookupResult(
                profile.Id,
                profile.UserId,
                profile.Status,
                profile.AvailabilityStatus))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TechnicianPublicLookupResult?> GetPublicProfileAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles
            .AsNoTracking()
            .Where(profile => profile.Id == technicianId)
            .Select(profile => new TechnicianPublicLookupResult(
                profile.Id,
                profile.UserId,
                profile.FullName,
                profile.ProfileImageUrl,
                profile.AverageRating,
                profile.TotalReviews))
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

    public async Task<Guid?> GetTechnicianIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianProfiles
            .AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .Select(profile => (Guid?)profile.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateRatingAsync(Guid technicianId, decimal averageRating, int totalReviews, CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.TechnicianProfiles.FirstOrDefaultAsync(profile => profile.Id == technicianId, cancellationToken);
        if (profile is null)
        {
            return;
        }

        profile.AverageRating = averageRating;
        profile.TotalReviews = totalReviews;
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
