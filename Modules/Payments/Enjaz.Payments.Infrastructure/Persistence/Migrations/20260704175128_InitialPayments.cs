using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Payments.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payments");

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                schema: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    provider_transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    provider_order_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    transaction_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    raw_payload_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_webhook_logs",
                schema: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    event_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    provider_transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    provider_order_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    raw_payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    headers_json = table.Column<string>(type: "jsonb", nullable: true),
                    signature = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    processing_error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    received_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_webhook_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_snapshot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    checkout_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    provider_order_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    provider_payment_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    provider_transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    paid_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refund_requests",
                schema: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refund_requests", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_payment_id",
                schema: "payments",
                table: "payment_transactions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_provider_transaction_id",
                schema: "payments",
                table: "payment_transactions",
                column: "provider_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_webhook_logs_provider_transaction_id",
                schema: "payments",
                table: "payment_webhook_logs",
                column: "provider_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_webhook_logs_received_at_utc",
                schema: "payments",
                table: "payment_webhook_logs",
                column: "received_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_payments_customer_user_id",
                schema: "payments",
                table: "payments",
                column: "customer_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_job_id",
                schema: "payments",
                table: "payments",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_provider_order_id",
                schema: "payments",
                table: "payments",
                column: "provider_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_provider_transaction_id",
                schema: "payments",
                table: "payments",
                column: "provider_transaction_id",
                unique: true,
                filter: "provider_transaction_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_payments_status",
                schema: "payments",
                table: "payments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_refund_requests_job_id",
                schema: "payments",
                table: "refund_requests",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_refund_requests_payment_id",
                schema: "payments",
                table: "refund_requests",
                column: "payment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_transactions",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payment_webhook_logs",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "refund_requests",
                schema: "payments");
        }
    }
}
