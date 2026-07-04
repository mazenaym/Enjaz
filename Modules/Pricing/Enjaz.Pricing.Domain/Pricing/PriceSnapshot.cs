namespace Enjaz.Pricing.Domain.Pricing;

public sealed class PriceSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public Guid ServiceId { get; set; }
    public int ComplexityId { get; set; }
    public Guid? PricingRuleId { get; set; }
    public Guid? CommissionSettingId { get; set; }
    public Guid? VatSettingId { get; set; }
    public Guid? DepositRuleId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TechnicianPayoutAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public string Currency { get; set; } = "EGP";
    public bool RequiresInspection { get; set; }
    public string? BreakdownJson { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; }
}
