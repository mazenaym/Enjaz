using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Support.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "support");

            migrationBuilder.CreateTable(
                name: "job_disputes",
                schema: "support",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opened_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    resolution = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_disputes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket_messages",
                schema: "support",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "support_tickets",
                schema: "support",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    related_job_id = table.Column<Guid>(type: "uuid", nullable: true),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    subject = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    assigned_admin_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    closed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_tickets", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_disputes_job_id",
                schema: "support",
                table: "job_disputes",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_disputes_status",
                schema: "support",
                table: "job_disputes",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_messages_sender_user_id",
                schema: "support",
                table: "support_ticket_messages",
                column: "sender_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_messages_ticket_id",
                schema: "support",
                table: "support_ticket_messages",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_created_by_user_id",
                schema: "support",
                table: "support_tickets",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_priority",
                schema: "support",
                table: "support_tickets",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_status",
                schema: "support",
                table: "support_tickets",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_ticket_number",
                schema: "support",
                table: "support_tickets",
                column: "ticket_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_disputes",
                schema: "support");

            migrationBuilder.DropTable(
                name: "support_ticket_messages",
                schema: "support");

            migrationBuilder.DropTable(
                name: "support_tickets",
                schema: "support");
        }
    }
}
