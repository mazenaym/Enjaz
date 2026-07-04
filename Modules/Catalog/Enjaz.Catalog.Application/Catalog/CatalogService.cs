using Enjaz.Catalog.Domain.Catalog;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Catalog.Application.Catalog;

public sealed class CatalogService(ICatalogRepository repository) : ICatalogService
{
    public async Task<Result<IReadOnlyCollection<ServiceCategoryResponse>>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await repository.GetActiveCategoriesAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<ServiceCategoryResponse>>(categories.Select(Map).ToArray());
    }

    public async Task<Result<IReadOnlyCollection<ServiceResponse>>> GetActiveServicesAsync(CancellationToken cancellationToken = default)
    {
        var services = await repository.GetActiveServicesAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<ServiceResponse>>(services.Select(Map).ToArray());
    }

    public async Task<Result<IReadOnlyCollection<ServiceResponse>>> GetServicesByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetCategoryAsync(categoryId, cancellationToken);
        if (category is null || !category.IsActive)
        {
            return Result.Failure<IReadOnlyCollection<ServiceResponse>>("category_not_found", "Service category was not found.");
        }

        var services = await repository.GetActiveServicesByCategoryAsync(categoryId, cancellationToken);
        return Result.Success<IReadOnlyCollection<ServiceResponse>>(services.Select(Map).ToArray());
    }

    public async Task<Result<ServiceResponse>> GetServiceDetailsAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        var service = await repository.GetServiceWithTiersAsync(serviceId, cancellationToken);
        if (service is null || !service.IsActive || service.Category?.IsActive != true)
        {
            return Result.Failure<ServiceResponse>("service_not_found", "Service was not found.");
        }

        return Result.Success(Map(service));
    }

    public async Task<Result<ServiceCategoryResponse>> CreateCategoryAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.CategorySlugExistsAsync(request.Slug, cancellationToken: cancellationToken))
        {
            return Result.Failure<ServiceCategoryResponse>("category_slug_exists", "Category slug already exists.");
        }

        var category = new ServiceCategory { CreatedAtUtc = DateTime.UtcNow };
        Apply(category, request);
        await repository.AddCategoryAsync(category, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(category));
    }

    public async Task<Result<ServiceCategoryResponse>> UpdateCategoryAsync(Guid id, CategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetCategoryAsync(id, cancellationToken);
        if (category is null)
        {
            return Result.Failure<ServiceCategoryResponse>("category_not_found", "Service category was not found.");
        }

        if (await repository.CategorySlugExistsAsync(request.Slug, id, cancellationToken))
        {
            return Result.Failure<ServiceCategoryResponse>("category_slug_exists", "Category slug already exists.");
        }

        Apply(category, request);
        category.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(category));
    }

    public async Task<Result> SetCategoryActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetCategoryAsync(id, cancellationToken);
        if (category is null)
        {
            return Result.Failure("category_not_found", "Service category was not found.");
        }

        category.IsActive = isActive;
        category.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<ServiceResponse>> CreateServiceAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.GetCategoryAsync(request.CategoryId, cancellationToken) is null)
        {
            return Result.Failure<ServiceResponse>("category_not_found", "Service category was not found.");
        }

        if (await repository.ServiceSlugExistsAsync(request.Slug, cancellationToken: cancellationToken))
        {
            return Result.Failure<ServiceResponse>("service_slug_exists", "Service slug already exists.");
        }

        var service = new Service { CreatedAtUtc = DateTime.UtcNow };
        Apply(service, request);
        await repository.AddServiceAsync(service, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(service));
    }

    public async Task<Result<ServiceResponse>> UpdateServiceAsync(Guid id, ServiceRequest request, CancellationToken cancellationToken = default)
    {
        var service = await repository.GetServiceWithTiersAsync(id, cancellationToken);
        if (service is null)
        {
            return Result.Failure<ServiceResponse>("service_not_found", "Service was not found.");
        }

        if (await repository.GetCategoryAsync(request.CategoryId, cancellationToken) is null)
        {
            return Result.Failure<ServiceResponse>("category_not_found", "Service category was not found.");
        }

        if (await repository.ServiceSlugExistsAsync(request.Slug, id, cancellationToken))
        {
            return Result.Failure<ServiceResponse>("service_slug_exists", "Service slug already exists.");
        }

        Apply(service, request);
        service.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(service));
    }

    public async Task<Result> SetServiceActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var service = await repository.GetServiceAsync(id, cancellationToken);
        if (service is null)
        {
            return Result.Failure("service_not_found", "Service was not found.");
        }

        service.IsActive = isActive;
        service.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<ServiceTierResponse>> CreateServiceTierAsync(ServiceTierRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.GetServiceAsync(request.ServiceId, cancellationToken) is null)
        {
            return Result.Failure<ServiceTierResponse>("service_not_found", "Service was not found.");
        }

        var tier = new ServiceTier { CreatedAtUtc = DateTime.UtcNow };
        Apply(tier, request);
        await repository.AddServiceTierAsync(tier, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(tier));
    }

    public async Task<Result<ServiceTierResponse>> UpdateServiceTierAsync(Guid id, ServiceTierRequest request, CancellationToken cancellationToken = default)
    {
        var tier = await repository.GetServiceTierAsync(id, cancellationToken);
        if (tier is null)
        {
            return Result.Failure<ServiceTierResponse>("service_tier_not_found", "Service tier was not found.");
        }

        if (await repository.GetServiceAsync(request.ServiceId, cancellationToken) is null)
        {
            return Result.Failure<ServiceTierResponse>("service_not_found", "Service was not found.");
        }

        Apply(tier, request);
        tier.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(tier));
    }

    public async Task<Result> SetServiceTierActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var tier = await repository.GetServiceTierAsync(id, cancellationToken);
        if (tier is null)
        {
            return Result.Failure("service_tier_not_found", "Service tier was not found.");
        }

        tier.IsActive = isActive;
        tier.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static void Apply(ServiceCategory category, CategoryRequest request)
    {
        category.NameAr = request.NameAr.Trim();
        category.NameEn = TrimOptional(request.NameEn);
        category.Slug = request.Slug.Trim();
        category.DescriptionAr = TrimOptional(request.DescriptionAr);
        category.DescriptionEn = TrimOptional(request.DescriptionEn);
        category.IconUrl = TrimOptional(request.IconUrl);
        category.DisplayOrder = request.DisplayOrder;
    }

    private static void Apply(Service service, ServiceRequest request)
    {
        service.CategoryId = request.CategoryId;
        service.NameAr = request.NameAr.Trim();
        service.NameEn = TrimOptional(request.NameEn);
        service.Slug = request.Slug.Trim();
        service.DescriptionAr = TrimOptional(request.DescriptionAr);
        service.DescriptionEn = TrimOptional(request.DescriptionEn);
        service.IconUrl = TrimOptional(request.IconUrl);
        service.DisplayOrder = request.DisplayOrder;
    }

    private static void Apply(ServiceTier tier, ServiceTierRequest request)
    {
        tier.ServiceId = request.ServiceId;
        tier.Name = request.Name.Trim();
        tier.DescriptionAr = TrimOptional(request.DescriptionAr);
        tier.DescriptionEn = TrimOptional(request.DescriptionEn);
        tier.DisplayOrder = request.DisplayOrder;
    }

    private static ServiceCategoryResponse Map(ServiceCategory category)
    {
        return new ServiceCategoryResponse(category.Id, category.NameAr, category.NameEn, category.Slug, category.DescriptionAr, category.DescriptionEn, category.IconUrl, category.DisplayOrder, category.IsActive);
    }

    private static ServiceResponse Map(Service service)
    {
        return new ServiceResponse(service.Id, service.CategoryId, service.NameAr, service.NameEn, service.Slug, service.DescriptionAr, service.DescriptionEn, service.IconUrl, service.IsActive, service.DisplayOrder, service.Tiers.Where(tier => tier.IsActive).OrderBy(tier => tier.DisplayOrder).Select(Map).ToArray());
    }

    private static ServiceTierResponse Map(ServiceTier tier)
    {
        return new ServiceTierResponse(tier.Id, tier.ServiceId, tier.Name, tier.DescriptionAr, tier.DescriptionEn, tier.IsActive, tier.DisplayOrder);
    }

    private static string? TrimOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
