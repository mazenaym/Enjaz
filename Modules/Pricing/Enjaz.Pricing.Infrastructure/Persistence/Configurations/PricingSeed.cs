using Enjaz.Pricing.Domain.Pricing;

namespace Enjaz.Pricing.Infrastructure.Persistence.Configurations;

internal static class PricingSeed
{
    private static readonly DateTime SeededAtUtc = new(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc);

    internal static readonly CommissionSetting[] CommissionSettings =
    [
        new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), Name = "Default commission 15%", CommissionRate = 0.15m, IsDefault = true, IsActive = true, EffectiveFromUtc = SeededAtUtc, CreatedAtUtc = SeededAtUtc }
    ];

    internal static readonly VatSetting[] VatSettings =
    [
        new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), Name = "Default VAT 14%", VatRate = 0.14m, AppliesOn = "Commission", IsDefault = true, IsActive = true, EffectiveFromUtc = SeededAtUtc, CreatedAtUtc = SeededAtUtc }
    ];

    internal static readonly DepositRule[] DepositRules =
    [
        new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), Name = "Default deposit 20%", DepositType = "Percentage", DepositValue = 0.20m, IsDefault = true, IsActive = true, CreatedAtUtc = SeededAtUtc }
    ];

    internal static readonly PricingRule[] PricingRules = BuildPricingRules();

    private static PricingRule[] BuildPricingRules()
    {
        var services = new (Guid CategoryId, Guid ServiceId)[]
        {
            (Guid.Parse("10000000-0000-0000-0000-000000000001"), Guid.Parse("20000000-0000-0000-0000-000000000001")),
            (Guid.Parse("10000000-0000-0000-0000-000000000001"), Guid.Parse("20000000-0000-0000-0000-000000000002")),
            (Guid.Parse("10000000-0000-0000-0000-000000000002"), Guid.Parse("20000000-0000-0000-0000-000000000003")),
            (Guid.Parse("10000000-0000-0000-0000-000000000002"), Guid.Parse("20000000-0000-0000-0000-000000000004")),
            (Guid.Parse("10000000-0000-0000-0000-000000000003"), Guid.Parse("20000000-0000-0000-0000-000000000005")),
            (Guid.Parse("10000000-0000-0000-0000-000000000003"), Guid.Parse("20000000-0000-0000-0000-000000000006")),
            (Guid.Parse("10000000-0000-0000-0000-000000000004"), Guid.Parse("20000000-0000-0000-0000-000000000007")),
            (Guid.Parse("10000000-0000-0000-0000-000000000004"), Guid.Parse("20000000-0000-0000-0000-000000000008")),
            (Guid.Parse("10000000-0000-0000-0000-000000000005"), Guid.Parse("20000000-0000-0000-0000-000000000009")),
            (Guid.Parse("10000000-0000-0000-0000-000000000005"), Guid.Parse("20000000-0000-0000-0000-000000000010")),
            (Guid.Parse("10000000-0000-0000-0000-000000000006"), Guid.Parse("20000000-0000-0000-0000-000000000011")),
            (Guid.Parse("10000000-0000-0000-0000-000000000006"), Guid.Parse("20000000-0000-0000-0000-000000000012"))
        };

        return services.SelectMany((service, index) => new[]
        {
            Rule(index, service.CategoryId, service.ServiceId, 1, 150m, false),
            Rule(index, service.CategoryId, service.ServiceId, 2, 250m, false),
            Rule(index, service.CategoryId, service.ServiceId, 3, 0m, true)
        }).ToArray();
    }

    private static PricingRule Rule(int serviceIndex, Guid categoryId, Guid serviceId, int complexityId, decimal basePrice, bool requiresInspection)
    {
        return new PricingRule
        {
            Id = Guid.Parse($"51000000-0000-{serviceIndex + 1:D4}-000{complexityId}-000000000001"),
            ServiceCategoryId = categoryId,
            ServiceId = serviceId,
            ComplexityId = complexityId,
            BasePrice = basePrice,
            Currency = "EGP",
            RequiresInspection = requiresInspection,
            IsActive = true,
            EffectiveFromUtc = SeededAtUtc,
            CreatedAtUtc = SeededAtUtc
        };
    }
}
