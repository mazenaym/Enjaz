using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Enjaz.Notifications.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notifications");

            migrationBuilder.CreateTable(
                name: "notification_delivery_logs",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    notification_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    provider = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    recipient = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_attempt_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    next_retry_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    request_json = table.Column<string>(type: "jsonb", nullable: true),
                    response_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_delivery_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_templates",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "ar"),
                    title_template = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body_template = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    data_json = table.Column<string>(type: "jsonb", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    read_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "push_device_tokens",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    platform = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    provider = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    device_id = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_seen_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_push_device_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_notification_preferences",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_notification_preferences", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "notifications",
                table: "notification_templates",
                columns: new[] { "id", "body_template", "channel", "created_at_utc", "is_active", "language", "title_template", "type", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000801"), "تم إنشاء طلبك رقم {{jobNumber}} بنجاح.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم إنشاء الطلب", "JobCreated", null },
                    { new Guid("10000000-0000-0000-0000-000000000802"), "تم تحديث حالة طلبك رقم {{jobNumber}} إلى {{status}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تحديث حالة الطلب", "JobStatusChanged", null },
                    { new Guid("10000000-0000-0000-0000-000000000803"), "تم تعيين فني لطلبك رقم {{jobNumber}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم تعيين فني", "TechnicianAssigned", null },
                    { new Guid("10000000-0000-0000-0000-000000000804"), "تم قبول المهمة رقم {{jobNumber}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم قبول المهمة", "AssignmentAccepted", null },
                    { new Guid("10000000-0000-0000-0000-000000000805"), "تم رفض المهمة رقم {{jobNumber}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم رفض المهمة", "AssignmentRejected", null },
                    { new Guid("10000000-0000-0000-0000-000000000806"), "تم إلغاء طلبك رقم {{jobNumber}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم إلغاء الطلب", "JobCancelled", null },
                    { new Guid("10000000-0000-0000-0000-000000000807"), "تم إنشاء رابط الدفع لطلبك رقم {{jobNumber}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم إنشاء رابط الدفع", "PaymentCheckoutCreated", null },
                    { new Guid("10000000-0000-0000-0000-000000000808"), "تم استلام دفعتك لطلب رقم {{jobNumber}} بمبلغ {{amount}} {{currency}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم الدفع بنجاح", "PaymentSucceeded", null },
                    { new Guid("10000000-0000-0000-0000-000000000809"), "تعذر إتمام الدفع لطلب رقم {{jobNumber}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "فشل الدفع", "PaymentFailed", null },
                    { new Guid("10000000-0000-0000-0000-000000000810"), "تم تحديث محفظتك بمبلغ {{amount}} {{currency}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "تم تحديث المحفظة", "WalletUpdated", null },
                    { new Guid("10000000-0000-0000-0000-000000000811"), "لديك ربح قيد الانتظار بمبلغ {{amount}} {{currency}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "ربح قيد الانتظار", "TechnicianEarningPending", null },
                    { new Guid("10000000-0000-0000-0000-000000000812"), "أصبح ربحك متاحا بمبلغ {{amount}} {{currency}}.", "InApp", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "ar", "ربح متاح", "TechnicianEarningAvailable", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_delivery_logs_next_retry_at_utc",
                schema: "notifications",
                table: "notification_delivery_logs",
                column: "next_retry_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_notification_delivery_logs_status",
                schema: "notifications",
                table: "notification_delivery_logs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_notification_templates_type_channel_language",
                schema: "notifications",
                table: "notification_templates",
                columns: new[] { "type", "channel", "language" },
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_created_at_utc",
                schema: "notifications",
                table: "notifications",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_is_read",
                schema: "notifications",
                table: "notifications",
                column: "is_read");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_type",
                schema: "notifications",
                table: "notifications",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                schema: "notifications",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_push_device_tokens_token",
                schema: "notifications",
                table: "push_device_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_push_device_tokens_user_id",
                schema: "notifications",
                table: "push_device_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_notification_preferences_user_id_channel",
                schema: "notifications",
                table: "user_notification_preferences",
                columns: new[] { "user_id", "channel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_delivery_logs",
                schema: "notifications");

            migrationBuilder.DropTable(
                name: "notification_templates",
                schema: "notifications");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "notifications");

            migrationBuilder.DropTable(
                name: "push_device_tokens",
                schema: "notifications");

            migrationBuilder.DropTable(
                name: "user_notification_preferences",
                schema: "notifications");
        }
    }
}
