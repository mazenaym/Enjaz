namespace Enjaz.Catalog.Domain.Catalog;

public sealed class ServiceCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
