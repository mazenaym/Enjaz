using Enjaz.Catalog.Application.Catalog;
using Enjaz.Catalog.Domain.Catalog;
using Enjaz.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Catalog.Infrastructure.Catalog;

public sealed class CatalogRepository(CatalogDbContext dbContext) : ICatalogRepository
{
    public async Task<IReadOnlyCollection<ServiceCategory>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceCategories
            .Where(category => category.IsActive)
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.NameEn)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Service>> GetActiveServicesAsync(CancellationToken cancellationToken = default)
    {
        return await ActiveServices()
            .OrderBy(service => service.DisplayOrder)
            .ThenBy(service => service.NameEn)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Service>> GetActiveServicesByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await ActiveServices()
            .Where(service => service.CategoryId == categoryId)
            .OrderBy(service => service.DisplayOrder)
            .ThenBy(service => service.NameEn)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Service?> GetServiceWithTiersAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Services
            .Include(service => service.Category)
            .Include(service => service.Tiers)
            .FirstOrDefaultAsync(service => service.Id == serviceId, cancellationToken);
    }

    public async Task<ServiceCategory?> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceCategories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<Service?> GetServiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Services.FirstOrDefaultAsync(service => service.Id == id, cancellationToken);
    }

    public async Task<ServiceTier?> GetServiceTierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceTiers.FirstOrDefaultAsync(tier => tier.Id == id, cancellationToken);
    }

    public async Task<bool> CategorySlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceCategories.AnyAsync(
            category => category.Slug == slug && (excludingId == null || category.Id != excludingId),
            cancellationToken);
    }

    public async Task<bool> ServiceSlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        return await dbContext.Services.AnyAsync(
            service => service.Slug == slug && (excludingId == null || service.Id != excludingId),
            cancellationToken);
    }

    public async Task AddCategoryAsync(ServiceCategory category, CancellationToken cancellationToken = default)
    {
        await dbContext.ServiceCategories.AddAsync(category, cancellationToken);
    }

    public async Task AddServiceAsync(Service service, CancellationToken cancellationToken = default)
    {
        await dbContext.Services.AddAsync(service, cancellationToken);
    }

    public async Task AddServiceTierAsync(ServiceTier tier, CancellationToken cancellationToken = default)
    {
        await dbContext.ServiceTiers.AddAsync(tier, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Service> ActiveServices()
    {
        return dbContext.Services
            .Include(service => service.Category)
            .Include(service => service.Tiers)
            .Where(service => service.IsActive && service.Category != null && service.Category.IsActive);
    }
}
