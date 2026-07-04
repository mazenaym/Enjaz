using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Technicians.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialTechnicians : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "technicians");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "technician_profiles",
                schema: "technicians",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    national_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    profile_image_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    bio = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    years_of_experience = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    availability_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    total_reviews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejected_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "technician_availability_history",
                schema: "technicians",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    to_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    changed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_availability_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_technician_availability_history_technician_profiles_technic~",
                        column: x => x.technician_id,
                        principalSchema: "technicians",
                        principalTable: "technician_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "technician_documents",
                schema: "technicians",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    file_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    rejection_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    uploaded_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reviewed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_technician_documents_technician_profiles_technician_id",
                        column: x => x.technician_id,
                        principalSchema: "technicians",
                        principalTable: "technician_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "technician_skills",
                schema: "technicians",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_skills", x => x.id);
                    table.ForeignKey(
                        name: "FK_technician_skills_technician_profiles_technician_id",
                        column: x => x.technician_id,
                        principalSchema: "technicians",
                        principalTable: "technician_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_technician_availability_history_technician_id",
                schema: "technicians",
                table: "technician_availability_history",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_documents_technician_id",
                schema: "technicians",
                table: "technician_documents",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_profiles_availability_status",
                schema: "technicians",
                table: "technician_profiles",
                column: "availability_status");

            migrationBuilder.CreateIndex(
                name: "IX_technician_profiles_status",
                schema: "technicians",
                table: "technician_profiles",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_technician_profiles_user_id",
                schema: "technicians",
                table: "technician_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_technician_skills_service_category_id",
                schema: "technicians",
                table: "technician_skills",
                column: "service_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_skills_service_id",
                schema: "technicians",
                table: "technician_skills",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_skills_technician_id_service_id",
                schema: "technicians",
                table: "technician_skills",
                columns: new[] { "technician_id", "service_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "technician_availability_history",
                schema: "technicians");

            migrationBuilder.DropTable(
                name: "technician_documents",
                schema: "technicians");

            migrationBuilder.DropTable(
                name: "technician_skills",
                schema: "technicians");

            migrationBuilder.DropTable(
                name: "technician_profiles",
                schema: "technicians");
        }
    }
}
