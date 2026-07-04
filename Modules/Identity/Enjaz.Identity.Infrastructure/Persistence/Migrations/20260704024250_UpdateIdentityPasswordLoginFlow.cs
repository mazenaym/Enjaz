using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjaz.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityPasswordLoginFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_email_verified",
                schema: "identity",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "normalized_email",
                schema: "identity",
                table: "users",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_username",
                schema: "identity",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                schema: "identity",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "username",
                schema: "identity",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_normalized_email",
                schema: "identity",
                table: "users",
                column: "normalized_email",
                unique: true,
                filter: "normalized_email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_normalized_username",
                schema: "identity",
                table: "users",
                column: "normalized_username",
                unique: true,
                filter: "normalized_username IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_normalized_email",
                schema: "identity",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_normalized_username",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_email_verified",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "normalized_email",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "normalized_username",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_hash",
                schema: "identity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "username",
                schema: "identity",
                table: "users");
        }
    }
}
