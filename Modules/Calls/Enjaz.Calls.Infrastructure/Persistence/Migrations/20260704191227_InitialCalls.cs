using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Calls.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "calls");

            migrationBuilder.CreateTable(
                name: "call_sessions",
                schema: "calls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    initiated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    provider_call_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    masked_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ended_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    recording_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_call_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "call_webhook_logs",
                schema: "calls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    provider_call_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    raw_payload_json = table.Column<string>(type: "text", nullable: false),
                    headers_json = table.Column<string>(type: "text", nullable: true),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    processing_error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    received_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_call_webhook_logs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_call_sessions_job_id",
                schema: "calls",
                table: "call_sessions",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_sessions_provider_call_id",
                schema: "calls",
                table: "call_sessions",
                column: "provider_call_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_sessions_status",
                schema: "calls",
                table: "call_sessions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_call_webhook_logs_provider_call_id",
                schema: "calls",
                table: "call_webhook_logs",
                column: "provider_call_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_webhook_logs_received_at_utc",
                schema: "calls",
                table: "call_webhook_logs",
                column: "received_at_utc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "call_sessions",
                schema: "calls");

            migrationBuilder.DropTable(
                name: "call_webhook_logs",
                schema: "calls");
        }
    }
}
