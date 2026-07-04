namespace Enjaz.Pricing.Domain.Pricing;

public sealed class DepositRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string DepositType { get; set; } = "Percentage";
    public decimal DepositValue { get; set; }
    public decimal? MinimumDeposit { get; set; }
    public decimal? MaximumDeposit { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
