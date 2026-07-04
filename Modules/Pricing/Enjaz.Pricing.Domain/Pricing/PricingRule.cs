namespace Enjaz.Pricing.Domain.Pricing;

public sealed class PricingRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServiceCategoryId { get; set; }
    public Guid ServiceId { get; set; }
    public int ComplexityId { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "EGP";
    public bool RequiresInspection { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
