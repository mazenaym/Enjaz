using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Enjaz.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "service_categories",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name_ar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name_en = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description_ar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    description_en = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    icon_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "services",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name_ar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name_en = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description_ar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    description_en = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    icon_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.id);
                    table.ForeignKey(
                        name: "FK_services_service_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "catalog",
                        principalTable: "service_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_tiers",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description_ar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    description_en = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_tiers", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_tiers_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "catalog",
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "service_categories",
                columns: new[] { "id", "created_at_utc", "description_ar", "description_en", "display_order", "icon_url", "is_active", "name_ar", "name_en", "slug", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, null, true, "سباكة", "Plumbing", "plumbing", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, null, true, "كهرباء", "Electrical", "electrical", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, null, true, "تكييف", "AC", "ac", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 4, null, true, "نجارة", "Carpentry", "carpentry", null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 5, null, true, "دهانات", "Painting", "painting", null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 6, null, true, "أجهزة منزلية", "Appliances", "appliances", null }
                });

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "services",
                columns: new[] { "id", "category_id", "created_at_utc", "description_ar", "description_en", "display_order", "icon_url", "is_active", "name_ar", "name_en", "slug", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, null, true, "إصلاح تسريب حنفية", "Faucet leak repair", "faucet-leak-repair", null },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, null, true, "كشف تسريب مياه", "Water leakage", "water-leakage", null },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, null, true, "إصلاح بريزة كهرباء", "Electrical outlet repair", "electrical-outlet-repair", null },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, null, true, "تركيب مفتاح كهرباء", "Switch installation", "switch-installation", null },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, null, true, "التكييف لا يبرد", "AC not cooling", "ac-not-cooling", null },
                    { new Guid("20000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, null, true, "تنظيف وصيانة تكييف", "AC cleaning and maintenance", "ac-cleaning-maintenance", null },
                    { new Guid("20000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, null, true, "إصلاح باب", "Door repair", "door-repair", null },
                    { new Guid("20000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, null, true, "تركيب رفوف", "Shelf installation", "shelf-installation", null },
                    { new Guid("20000000-0000-0000-0000-000000000009"), new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, null, true, "دهان غرفة", "Room painting", "room-painting", null },
                    { new Guid("20000000-0000-0000-0000-000000000010"), new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, null, true, "معالجة رطوبة", "Moisture treatment", "moisture-treatment", null },
                    { new Guid("20000000-0000-0000-0000-000000000011"), new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, null, true, "إصلاح غسالة", "Washing machine repair", "washing-machine-repair", null },
                    { new Guid("20000000-0000-0000-0000-000000000012"), new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, null, true, "إصلاح ثلاجة", "Refrigerator repair", "refrigerator-repair", null }
                });

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "service_tiers",
                columns: new[] { "id", "created_at_utc", "description_ar", "description_en", "display_order", "is_active", "name", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0001-0000-000000000001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000001"), null },
                    { new Guid("30000000-0000-0001-0000-000000000002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000002"), null },
                    { new Guid("30000000-0000-0001-0000-000000000003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000003"), null },
                    { new Guid("30000000-0000-0001-0000-000000000004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000004"), null },
                    { new Guid("30000000-0000-0001-0000-000000000005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000005"), null },
                    { new Guid("30000000-0000-0001-0000-000000000006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000006"), null },
                    { new Guid("30000000-0000-0001-0000-000000000007"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000007"), null },
                    { new Guid("30000000-0000-0001-0000-000000000008"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000008"), null },
                    { new Guid("30000000-0000-0001-0000-000000000009"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000009"), null },
                    { new Guid("30000000-0000-0001-0000-000000000010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000010"), null },
                    { new Guid("30000000-0000-0001-0000-000000000011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000011"), null },
                    { new Guid("30000000-0000-0001-0000-000000000012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, true, "Simple", new Guid("20000000-0000-0000-0000-000000000012"), null },
                    { new Guid("30000000-0000-0002-0000-000000000001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000001"), null },
                    { new Guid("30000000-0000-0002-0000-000000000002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000002"), null },
                    { new Guid("30000000-0000-0002-0000-000000000003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000003"), null },
                    { new Guid("30000000-0000-0002-0000-000000000004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000004"), null },
                    { new Guid("30000000-0000-0002-0000-000000000005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000005"), null },
                    { new Guid("30000000-0000-0002-0000-000000000006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000006"), null },
                    { new Guid("30000000-0000-0002-0000-000000000007"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000007"), null },
                    { new Guid("30000000-0000-0002-0000-000000000008"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000008"), null },
                    { new Guid("30000000-0000-0002-0000-000000000009"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000009"), null },
                    { new Guid("30000000-0000-0002-0000-000000000010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000010"), null },
                    { new Guid("30000000-0000-0002-0000-000000000011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000011"), null },
                    { new Guid("30000000-0000-0002-0000-000000000012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, true, "Standard", new Guid("20000000-0000-0000-0000-000000000012"), null },
                    { new Guid("30000000-0000-0003-0000-000000000001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000001"), null },
                    { new Guid("30000000-0000-0003-0000-000000000002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000002"), null },
                    { new Guid("30000000-0000-0003-0000-000000000003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000003"), null },
                    { new Guid("30000000-0000-0003-0000-000000000004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000004"), null },
                    { new Guid("30000000-0000-0003-0000-000000000005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000005"), null },
                    { new Guid("30000000-0000-0003-0000-000000000006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000006"), null },
                    { new Guid("30000000-0000-0003-0000-000000000007"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000007"), null },
                    { new Guid("30000000-0000-0003-0000-000000000008"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000008"), null },
                    { new Guid("30000000-0000-0003-0000-000000000009"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000009"), null },
                    { new Guid("30000000-0000-0003-0000-000000000010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000010"), null },
                    { new Guid("30000000-0000-0003-0000-000000000011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000011"), null },
                    { new Guid("30000000-0000-0003-0000-000000000012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, true, "Complex", new Guid("20000000-0000-0000-0000-000000000012"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_service_categories_slug",
                schema: "catalog",
                table: "service_categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_tiers_service_id",
                schema: "catalog",
                table: "service_tiers",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_services_category_id",
                schema: "catalog",
                table: "services",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_services_slug",
                schema: "catalog",
                table: "services",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_tiers",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "services",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "service_categories",
                schema: "catalog");
        }
    }
}
