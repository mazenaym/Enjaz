using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Reviews.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reviews");

            migrationBuilder.CreateTable(
                name: "review_analysis",
                schema: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    review_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sentiment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    confidence = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    keywords_json = table.Column<string>(type: "text", nullable: true),
                    requires_admin_attention = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    raw_ai_response_json = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_analysis", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "technician_rating_snapshots",
                schema: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    total_reviews = table.Column<int>(type: "integer", nullable: false),
                    last_review_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_rating_snapshots", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_review_analysis_requires_admin_attention",
                schema: "reviews",
                table: "review_analysis",
                column: "requires_admin_attention");

            migrationBuilder.CreateIndex(
                name: "IX_review_analysis_review_id",
                schema: "reviews",
                table: "review_analysis",
                column: "review_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_customer_user_id",
                schema: "reviews",
                table: "reviews",
                column: "customer_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_job_id",
                schema: "reviews",
                table: "reviews",
                column: "job_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_technician_id",
                schema: "reviews",
                table: "reviews",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_rating_snapshots_technician_id",
                schema: "reviews",
                table: "technician_rating_snapshots",
                column: "technician_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "review_analysis",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "reviews",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "technician_rating_snapshots",
                schema: "reviews");
        }
    }
}
