using Enjaz.Catalog.Domain.Catalog;

namespace Enjaz.Catalog.Infrastructure.Persistence.Configurations;

internal static class CatalogSeed
{
    private static readonly DateTime SeededAtUtc = new(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc);

    internal static readonly Guid PlumbingId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    internal static readonly Guid ElectricalId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    internal static readonly Guid AcId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    internal static readonly Guid CarpentryId = Guid.Parse("10000000-0000-0000-0000-000000000004");
    internal static readonly Guid PaintingId = Guid.Parse("10000000-0000-0000-0000-000000000005");
    internal static readonly Guid AppliancesId = Guid.Parse("10000000-0000-0000-0000-000000000006");

    internal static readonly ServiceCategory[] Categories =
    [
        Category(PlumbingId, "سباكة", "Plumbing", "plumbing", 1),
        Category(ElectricalId, "كهرباء", "Electrical", "electrical", 2),
        Category(AcId, "تكييف", "AC", "ac", 3),
        Category(CarpentryId, "نجارة", "Carpentry", "carpentry", 4),
        Category(PaintingId, "دهانات", "Painting", "painting", 5),
        Category(AppliancesId, "أجهزة منزلية", "Appliances", "appliances", 6)
    ];

    internal static readonly Service[] Services =
    [
        Service("20000000-0000-0000-0000-000000000001", PlumbingId, "إصلاح تسريب حنفية", "Faucet leak repair", "faucet-leak-repair", 1),
        Service("20000000-0000-0000-0000-000000000002", PlumbingId, "كشف تسريب مياه", "Water leakage", "water-leakage", 2),
        Service("20000000-0000-0000-0000-000000000003", ElectricalId, "إصلاح بريزة كهرباء", "Electrical outlet repair", "electrical-outlet-repair", 1),
        Service("20000000-0000-0000-0000-000000000004", ElectricalId, "تركيب مفتاح كهرباء", "Switch installation", "switch-installation", 2),
        Service("20000000-0000-0000-0000-000000000005", AcId, "التكييف لا يبرد", "AC not cooling", "ac-not-cooling", 1),
        Service("20000000-0000-0000-0000-000000000006", AcId, "تنظيف وصيانة تكييف", "AC cleaning and maintenance", "ac-cleaning-maintenance", 2),
        Service("20000000-0000-0000-0000-000000000007", CarpentryId, "إصلاح باب", "Door repair", "door-repair", 1),
        Service("20000000-0000-0000-0000-000000000008", CarpentryId, "تركيب رفوف", "Shelf installation", "shelf-installation", 2),
        Service("20000000-0000-0000-0000-000000000009", PaintingId, "دهان غرفة", "Room painting", "room-painting", 1),
        Service("20000000-0000-0000-0000-000000000010", PaintingId, "معالجة رطوبة", "Moisture treatment", "moisture-treatment", 2),
        Service("20000000-0000-0000-0000-000000000011", AppliancesId, "إصلاح غسالة", "Washing machine repair", "washing-machine-repair", 1),
        Service("20000000-0000-0000-0000-000000000012", AppliancesId, "إصلاح ثلاجة", "Refrigerator repair", "refrigerator-repair", 2)
    ];

    internal static readonly ServiceTier[] ServiceTiers = Services
        .SelectMany(service => new[]
        {
            Tier(service.Id, "Simple", 1),
            Tier(service.Id, "Standard", 2),
            Tier(service.Id, "Complex", 3)
        })
        .ToArray();

    private static ServiceCategory Category(Guid id, string nameAr, string nameEn, string slug, int order)
    {
        return new ServiceCategory { Id = id, NameAr = nameAr, NameEn = nameEn, Slug = slug, DisplayOrder = order, IsActive = true, CreatedAtUtc = SeededAtUtc };
    }

    private static Service Service(string id, Guid categoryId, string nameAr, string nameEn, string slug, int order)
    {
        return new Service { Id = Guid.Parse(id), CategoryId = categoryId, NameAr = nameAr, NameEn = nameEn, Slug = slug, DisplayOrder = order, IsActive = true, CreatedAtUtc = SeededAtUtc };
    }

    private static ServiceTier Tier(Guid serviceId, string name, int order)
    {
        var suffix = name switch
        {
            "Simple" => 1,
            "Standard" => 2,
            _ => 3
        };

        var serviceNumber = serviceId.ToString()[^12..];
        return new ServiceTier { Id = Guid.Parse($"30000000-0000-{suffix:D4}-0000-{serviceNumber}"), ServiceId = serviceId, Name = name, DisplayOrder = order, IsActive = true, CreatedAtUtc = SeededAtUtc };
    }
}
