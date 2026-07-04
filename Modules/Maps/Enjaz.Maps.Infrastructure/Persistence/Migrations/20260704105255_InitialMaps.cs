using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Enjaz.Maps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS postgis;");

            migrationBuilder.EnsureSchema(
                name: "maps");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "location_calculation_cache",
                schema: "maps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    origin_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    origin_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    destination_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    destination_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    distance_meters = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_calculation_cache", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_zones",
                schema: "maps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name_ar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name_en = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    city = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    area = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    polygon = table.Column<Polygon>(type: "geometry(Polygon,4326)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_zones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "technician_location_history",
                schema: "maps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    location = table.Column<Point>(type: "geometry(Point,4326)", nullable: false),
                    accuracy_meters = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    heading = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    speed_meters_per_second = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_location_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "technician_location_snapshots",
                schema: "maps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    location = table.Column<Point>(type: "geometry(Point,4326)", nullable: false),
                    accuracy_meters = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    heading = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    speed_meters_per_second = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_location_snapshots", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "maps",
                table: "service_zones",
                columns: new[] { "id", "area", "city", "created_at_utc", "is_active", "name_ar", "name_en", "polygon", "slug", "updated_at_utc" },
                values: new object[] { new Guid("40000000-0000-0000-0000-000000000001"), "Nasr City", "Cairo", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "Dev Cairo Zone", "Dev Cairo Zone", (NetTopologySuite.Geometries.Polygon)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POLYGON ((31.3 30.07, 31.35 30.08, 31.36 30.03, 31.31 30.02, 31.3 30.07))"), "dev-cairo-zone", null });

            migrationBuilder.CreateIndex(
                name: "IX_location_calculation_cache_expires_at_utc",
                schema: "maps",
                table: "location_calculation_cache",
                column: "expires_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_service_zones_is_active",
                schema: "maps",
                table: "service_zones",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_service_zones_polygon",
                schema: "maps",
                table: "service_zones",
                column: "polygon")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_service_zones_slug",
                schema: "maps",
                table: "service_zones",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_technician_location_history_created_at_utc",
                schema: "maps",
                table: "technician_location_history",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_technician_location_history_location",
                schema: "maps",
                table: "technician_location_history",
                column: "location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_technician_location_history_technician_id",
                schema: "maps",
                table: "technician_location_history",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_location_snapshots_location",
                schema: "maps",
                table: "technician_location_snapshots",
                column: "location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_technician_location_snapshots_technician_id",
                schema: "maps",
                table: "technician_location_snapshots",
                column: "technician_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "location_calculation_cache",
                schema: "maps");

            migrationBuilder.DropTable(
                name: "service_zones",
                schema: "maps");

            migrationBuilder.DropTable(
                name: "technician_location_history",
                schema: "maps");

            migrationBuilder.DropTable(
                name: "technician_location_snapshots",
                schema: "maps");
        }
    }
}
