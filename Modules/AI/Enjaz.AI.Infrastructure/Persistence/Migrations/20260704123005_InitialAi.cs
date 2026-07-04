using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.AI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ai");

            migrationBuilder.CreateTable(
                name: "ai_classifications",
                schema: "ai",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_description = table.Column<string>(type: "text", nullable: false),
                    complexity_id = table.Column<int>(type: "integer", nullable: false),
                    complexity_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    confidence = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    suggested_action = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    requires_inspection = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_classifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ai_requests",
                schema: "ai",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    feature = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    input_text = table.Column<string>(type: "text", nullable: false),
                    input_json = table.Column<string>(type: "jsonb", nullable: true),
                    raw_response_json = table.Column<string>(type: "jsonb", nullable: true),
                    success = table.Column<bool>(type: "boolean", nullable: false),
                    error_message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_requests", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_classifications_created_at_utc",
                schema: "ai",
                table: "ai_classifications",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_ai_classifications_service_category_id",
                schema: "ai",
                table: "ai_classifications",
                column: "service_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_ai_classifications_service_id",
                schema: "ai",
                table: "ai_classifications",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_created_at_utc",
                schema: "ai",
                table: "ai_requests",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_ai_requests_feature",
                schema: "ai",
                table: "ai_requests",
                column: "feature");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_classifications",
                schema: "ai");

            migrationBuilder.DropTable(
                name: "ai_requests",
                schema: "ai");
        }
    }
}
