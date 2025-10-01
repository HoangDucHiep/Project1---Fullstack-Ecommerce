using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "addresses",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "categories",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "shops",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "users",
            schema: "ecommerce-domain");
    }
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "ecommerce-domain");

        migrationBuilder.CreateTable(
            name: "categories",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                lft = table.Column<int>(type: "integer", nullable: false),
                rgt = table.Column<int>(type: "integer", nullable: false),
                depth = table.Column<int>(type: "integer", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_categories", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                identity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "addresses",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ward = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                address_line = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                is_default = table.Column<bool>(type: "boolean", nullable: false),
                is_pick_up_address = table.Column<bool>(type: "boolean", nullable: false),
                is_return_address = table.Column<bool>(type: "boolean", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_addresses", x => x.id);
                table.ForeignKey(
                    name: "fk_addresses_user_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "shops",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                banner_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_shops", x => x.id);
                table.ForeignKey(
                    name: "fk_shops_user_owner_id",
                    column: x => x.owner_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_addresses_user_id",
            schema: "ecommerce-domain",
            table: "addresses",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_categories_lft_rgt",
            schema: "ecommerce-domain",
            table: "categories",
            columns: ["lft", "rgt"]);

        migrationBuilder.CreateIndex(
            name: "ix_categories_name",
            schema: "ecommerce-domain",
            table: "categories",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ix_categories_parent_id",
            schema: "ecommerce-domain",
            table: "categories",
            column: "parent_id");

        migrationBuilder.CreateIndex(
            name: "ix_shops_owner_id",
            schema: "ecommerce-domain",
            table: "shops",
            column: "owner_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            schema: "ecommerce-domain",
            table: "users",
            column: "email",
            unique: true,
            filter: "\"email\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "ix_users_identity_id",
            schema: "ecommerce-domain",
            table: "users",
            column: "identity_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_phone",
            schema: "ecommerce-domain",
            table: "users",
            column: "phone",
            unique: true,
            filter: "\"phone\" IS NOT NULL");
    }
}
