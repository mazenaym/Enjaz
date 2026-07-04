using Enjaz.Catalog.Domain.Catalog;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Catalog.Application.Catalog;

public interface ICatalogService
{
    Task<Result<IReadOnlyCollection<ServiceCategoryResponse>>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ServiceResponse>>> GetActiveServicesAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ServiceResponse>>> GetServicesByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<ServiceResponse>> GetServiceDetailsAsync(Guid serviceId, CancellationToken cancellationToken = default);
    Task<Result<ServiceCategoryResponse>> CreateCategoryAsync(CategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<ServiceCategoryResponse>> UpdateCategoryAsync(Guid id, CategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetCategoryActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    Task<Result<ServiceResponse>> CreateServiceAsync(ServiceRequest request, CancellationToken cancellationToken = default);
    Task<Result<ServiceResponse>> UpdateServiceAsync(Guid id, ServiceRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetServiceActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    Task<Result<ServiceTierResponse>> CreateServiceTierAsync(ServiceTierRequest request, CancellationToken cancellationToken = default);
    Task<Result<ServiceTierResponse>> UpdateServiceTierAsync(Guid id, ServiceTierRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetServiceTierActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
}

public interface ICatalogRepository
{
    Task<IReadOnlyCollection<ServiceCategory>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Service>> GetActiveServicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Service>> GetActiveServicesByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Service?> GetServiceWithTiersAsync(Guid serviceId, CancellationToken cancellationToken = default);
    Task<ServiceCategory?> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Service?> GetServiceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceTier?> GetServiceTierAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CategorySlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default);
    Task<bool> ServiceSlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default);
    Task AddCategoryAsync(ServiceCategory category, CancellationToken cancellationToken = default);
    Task AddServiceAsync(Service service, CancellationToken cancellationToken = default);
    Task AddServiceTierAsync(ServiceTier tier, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
