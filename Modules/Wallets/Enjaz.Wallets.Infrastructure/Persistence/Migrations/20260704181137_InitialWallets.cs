using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Wallets.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialWallets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wallets");

            migrationBuilder.CreateTable(
                name: "ledger_entries",
                schema: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ledger_transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_direction = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    balance_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_entries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ledger_transactions",
                schema: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    source_module = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    source_entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payout_batch_items",
                schema: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payout_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    failure_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payout_batch_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payout_batches",
                schema: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payout_batches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "platform_earnings",
                schema: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_snapshot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: true),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    commission_rate = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: false),
                    commission_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    vat_rate = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: false),
                    vat_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_earnings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "technician_earnings",
                schema: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: true),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    available_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_earnings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                schema: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    available_balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    pending_balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    total_credited = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    total_debited = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallets", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ledger_entries_ledger_transaction_id",
                schema: "wallets",
                table: "ledger_entries",
                column: "ledger_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_entries_wallet_id",
                schema: "wallets",
                table: "ledger_entries",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_transactions_idempotency_key",
                schema: "wallets",
                table: "ledger_transactions",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ledger_transactions_source_module_source_entity_id",
                schema: "wallets",
                table: "ledger_transactions",
                columns: new[] { "source_module", "source_entity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_ledger_transactions_transaction_number",
                schema: "wallets",
                table: "ledger_transactions",
                column: "transaction_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payout_batch_items_payout_batch_id",
                schema: "wallets",
                table: "payout_batch_items",
                column: "payout_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_payout_batch_items_technician_id",
                schema: "wallets",
                table: "payout_batch_items",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_payout_batches_batch_number",
                schema: "wallets",
                table: "payout_batches",
                column: "batch_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_platform_earnings_job_id",
                schema: "wallets",
                table: "platform_earnings",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_platform_earnings_payment_id",
                schema: "wallets",
                table: "platform_earnings",
                column: "payment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_technician_earnings_job_id",
                schema: "wallets",
                table: "technician_earnings",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_earnings_status",
                schema: "wallets",
                table: "technician_earnings",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_technician_earnings_technician_id",
                schema: "wallets",
                table: "technician_earnings",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_owner_type_currency",
                schema: "wallets",
                table: "wallets",
                columns: new[] { "owner_type", "currency" },
                unique: true,
                filter: "is_active = true AND owner_user_id IS NULL AND technician_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_owner_type_owner_user_id_currency",
                schema: "wallets",
                table: "wallets",
                columns: new[] { "owner_type", "owner_user_id", "currency" },
                unique: true,
                filter: "is_active = true AND owner_user_id IS NOT NULL AND technician_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_owner_type_technician_id_currency",
                schema: "wallets",
                table: "wallets",
                columns: new[] { "owner_type", "technician_id", "currency" },
                unique: true,
                filter: "is_active = true AND technician_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_technician_id",
                schema: "wallets",
                table: "wallets",
                column: "technician_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ledger_entries",
                schema: "wallets");

            migrationBuilder.DropTable(
                name: "ledger_transactions",
                schema: "wallets");

            migrationBuilder.DropTable(
                name: "payout_batch_items",
                schema: "wallets");

            migrationBuilder.DropTable(
                name: "payout_batches",
                schema: "wallets");

            migrationBuilder.DropTable(
                name: "platform_earnings",
                schema: "wallets");

            migrationBuilder.DropTable(
                name: "technician_earnings",
                schema: "wallets");

            migrationBuilder.DropTable(
                name: "wallets",
                schema: "wallets");
        }
    }
}
