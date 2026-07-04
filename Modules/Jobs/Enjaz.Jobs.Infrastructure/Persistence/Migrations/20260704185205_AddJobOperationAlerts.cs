using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Jobs.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobOperationAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_operation_alerts",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    is_resolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_operation_alerts", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_operation_alerts_job_id_alert_type_is_resolved",
                schema: "jobs",
                table: "job_operation_alerts",
                columns: new[] { "job_id", "alert_type", "is_resolved" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_operation_alerts",
                schema: "jobs");
        }
    }
}
