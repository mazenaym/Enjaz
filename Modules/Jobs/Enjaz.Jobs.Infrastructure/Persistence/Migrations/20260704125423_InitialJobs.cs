using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Jobs.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "jobs");

            migrationBuilder.CreateTable(
                name: "job_assignments",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    offered_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_assignments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_counters",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    year_month = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    last_number = table.Column<int>(type: "integer", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_counters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_media",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    file_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    file_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_media", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_notes",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    note_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_notes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_status_history",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    to_status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_status_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_profile_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_address_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_tier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ai_classification_id = table.Column<Guid>(type: "uuid", nullable: true),
                    price_snapshot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_technician_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_technician_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    scheduled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    preferred_time_window_start_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    preferred_time_window_end_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    estimated_total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    estimated_deposit_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    requires_inspection = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cancelled_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_assignments_job_id",
                schema: "jobs",
                table: "job_assignments",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_assignments_status",
                schema: "jobs",
                table: "job_assignments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_job_assignments_technician_id",
                schema: "jobs",
                table: "job_assignments",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_counters_year_month",
                schema: "jobs",
                table: "job_counters",
                column: "year_month",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_media_job_id",
                schema: "jobs",
                table: "job_media",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_notes_job_id",
                schema: "jobs",
                table: "job_notes",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_status_history_job_id",
                schema: "jobs",
                table: "job_status_history",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_assigned_technician_id",
                schema: "jobs",
                table: "jobs",
                column: "assigned_technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_created_at_utc",
                schema: "jobs",
                table: "jobs",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_customer_address_id",
                schema: "jobs",
                table: "jobs",
                column: "customer_address_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_customer_user_id",
                schema: "jobs",
                table: "jobs",
                column: "customer_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_job_number",
                schema: "jobs",
                table: "jobs",
                column: "job_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_jobs_service_id",
                schema: "jobs",
                table: "jobs",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_status",
                schema: "jobs",
                table: "jobs",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_assignments",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "job_counters",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "job_media",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "job_notes",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "job_status_history",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "jobs",
                schema: "jobs");
        }
    }
}
