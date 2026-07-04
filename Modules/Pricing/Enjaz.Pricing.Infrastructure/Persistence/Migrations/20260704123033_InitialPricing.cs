using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Enjaz.Pricing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pricing");

            migrationBuilder.CreateTable(
                name: "commission_settings",
                schema: "pricing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    commission_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    effective_from_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    effective_to_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commission_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "deposit_rules",
                schema: "pricing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    deposit_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    deposit_value = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    minimum_deposit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    maximum_deposit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposit_rules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "price_snapshots",
                schema: "pricing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    complexity_id = table.Column<int>(type: "integer", nullable: false),
                    pricing_rule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    commission_setting_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vat_setting_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deposit_rule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    base_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    commission_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    commission_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    vat_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    vat_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    technician_payout_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    requires_inspection = table.Column<bool>(type: "boolean", nullable: false),
                    breakdown_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_snapshots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pricing_rules",
                schema: "pricing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    complexity_id = table.Column<int>(type: "integer", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    requires_inspection = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    effective_from_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    effective_to_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricing_rules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vat_settings",
                schema: "pricing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    vat_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    applies_on = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    effective_from_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    effective_to_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vat_settings", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "commission_settings",
                columns: new[] { "id", "commission_rate", "created_at_utc", "effective_from_utc", "effective_to_utc", "is_active", "is_default", "name" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000001"), 0.15m, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, "Default commission 15%" });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "deposit_rules",
                columns: new[] { "id", "created_at_utc", "deposit_type", "deposit_value", "is_active", "is_default", "maximum_deposit", "minimum_deposit", "name", "updated_at_utc" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Percentage", 0.20m, true, true, null, null, "Default deposit 20%", null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0001-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), null },
                    { new Guid("51000000-0000-0001-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0001-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0002-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002"), null },
                    { new Guid("51000000-0000-0002-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0002-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0003-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000003"), null },
                    { new Guid("51000000-0000-0003-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000003"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0003-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000003"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0004-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000004"), null },
                    { new Guid("51000000-0000-0004-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000004"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0004-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000004"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0005-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000005"), null },
                    { new Guid("51000000-0000-0005-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000005"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0005-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000005"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0006-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000006"), null },
                    { new Guid("51000000-0000-0006-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000006"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0006-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000006"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0007-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000007"), null },
                    { new Guid("51000000-0000-0007-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000007"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0007-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000007"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0008-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000008"), null },
                    { new Guid("51000000-0000-0008-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000008"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0008-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000008"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0009-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000009"), null },
                    { new Guid("51000000-0000-0009-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000009"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0009-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000009"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0010-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000010"), null },
                    { new Guid("51000000-0000-0010-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000010"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0010-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000010"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0011-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000011"), null },
                    { new Guid("51000000-0000-0011-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000011"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0011-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000011"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0012-0001-000000000001"), 150m, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000012"), null },
                    { new Guid("51000000-0000-0012-0002-000000000001"), 250m, 2, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000012"), null }
                });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "id", "base_price", "complexity_id", "created_at_utc", "currency", "effective_from_utc", "effective_to_utc", "is_active", "requires_inspection", "service_category_id", "service_id", "updated_at_utc" },
                values: new object[] { new Guid("51000000-0000-0012-0003-000000000001"), 0m, 3, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "EGP", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, new Guid("10000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000012"), null });

            migrationBuilder.InsertData(
                schema: "pricing",
                table: "vat_settings",
                columns: new[] { "id", "applies_on", "created_at_utc", "effective_from_utc", "effective_to_utc", "is_active", "is_default", "name", "vat_rate" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000002"), "Commission", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, "Default VAT 14%", 0.14m });

            migrationBuilder.CreateIndex(
                name: "IX_commission_settings_is_default",
                schema: "pricing",
                table: "commission_settings",
                column: "is_default",
                unique: true,
                filter: "is_default = TRUE AND is_active = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_rules_is_default",
                schema: "pricing",
                table: "deposit_rules",
                column: "is_default",
                unique: true,
                filter: "is_default = TRUE AND is_active = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_price_snapshots_created_at_utc",
                schema: "pricing",
                table: "price_snapshots",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_price_snapshots_service_id",
                schema: "pricing",
                table: "price_snapshots",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_snapshots_user_id",
                schema: "pricing",
                table: "price_snapshots",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_rules_effective_from_utc",
                schema: "pricing",
                table: "pricing_rules",
                column: "effective_from_utc");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_rules_service_id_complexity_id",
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "service_id", "complexity_id" },
                unique: true,
                filter: "is_active = TRUE AND effective_to_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_rules_service_id_complexity_id_is_active",
                schema: "pricing",
                table: "pricing_rules",
                columns: new[] { "service_id", "complexity_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_vat_settings_is_default",
                schema: "pricing",
                table: "vat_settings",
                column: "is_default",
                unique: true,
                filter: "is_default = TRUE AND is_active = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commission_settings",
                schema: "pricing");

            migrationBuilder.DropTable(
                name: "deposit_rules",
                schema: "pricing");

            migrationBuilder.DropTable(
                name: "price_snapshots",
                schema: "pricing");

            migrationBuilder.DropTable(
                name: "pricing_rules",
                schema: "pricing");

            migrationBuilder.DropTable(
                name: "vat_settings",
                schema: "pricing");
        }
    }
}
