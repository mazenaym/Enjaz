namespace Enjaz.Catalog.Application.Catalog;

public sealed record ServiceCategoryResponse(Guid Id, string NameAr, string? NameEn, string Slug, string? DescriptionAr, string? DescriptionEn, string? IconUrl, int DisplayOrder, bool IsActive);

public sealed record ServiceResponse(Guid Id, Guid CategoryId, string NameAr, string? NameEn, string Slug, string? DescriptionAr, string? DescriptionEn, string? IconUrl, bool IsActive, int DisplayOrder, IReadOnlyCollection<ServiceTierResponse> Tiers);

public sealed record ServiceTierResponse(Guid Id, Guid ServiceId, string Name, string? DescriptionAr, string? DescriptionEn, bool IsActive, int DisplayOrder);

public sealed record CategoryRequest(string NameAr, string? NameEn, string Slug, string? DescriptionAr, string? DescriptionEn, string? IconUrl, int DisplayOrder);

public sealed record ServiceRequest(Guid CategoryId, string NameAr, string? NameEn, string Slug, string? DescriptionAr, string? DescriptionEn, string? IconUrl, int DisplayOrder);

public sealed record ServiceTierRequest(Guid ServiceId, string Name, string? DescriptionAr, string? DescriptionEn, int DisplayOrder);
