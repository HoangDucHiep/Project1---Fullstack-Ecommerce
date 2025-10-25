using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class MergeUserAndUserProfileTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "user_profiles",
            schema: "ecommerce-domain");

        migrationBuilder.AddColumn<string>(
            name: "avatar_url",
            schema: "ecommerce-domain",
            table: "users",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "bio",
            schema: "ecommerce-domain",
            table: "users",
            type: "character varying(1000)",
            maxLength: 1000,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "id_card_full_address",
            schema: "ecommerce-domain",
            table: "users",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "id_card_full_name",
            schema: "ecommerce-domain",
            table: "users",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "id_card_number",
            schema: "ecommerce-domain",
            table: "users",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_status",
            schema: "ecommerce-domain",
            table: "users",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ix_users_user_name",
            schema: "ecommerce-domain",
            table: "users",
            column: "user_name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_users_status",
            schema: "ecommerce-domain",
            table: "users");

        migrationBuilder.DropIndex(
            name: "ix_users_user_name",
            schema: "ecommerce-domain",
            table: "users");

        migrationBuilder.DropColumn(
            name: "avatar_url",
            schema: "ecommerce-domain",
            table: "users");

        migrationBuilder.DropColumn(
            name: "bio",
            schema: "ecommerce-domain",
            table: "users");

        migrationBuilder.DropColumn(
            name: "id_card_full_address",
            schema: "ecommerce-domain",
            table: "users");

        migrationBuilder.DropColumn(
            name: "id_card_full_name",
            schema: "ecommerce-domain",
            table: "users");

        migrationBuilder.DropColumn(
            name: "id_card_number",
            schema: "ecommerce-domain",
            table: "users");

        migrationBuilder.CreateTable(
            name: "user_profiles",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                id_card_full_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                id_card_full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                id_card_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                user_id = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_profiles", x => x.id);
                table.ForeignKey(
                    name: "fk_user_profiles_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_user_profiles_user_id",
            schema: "ecommerce-domain",
            table: "user_profiles",
            column: "user_id",
            unique: true);
    }
}
