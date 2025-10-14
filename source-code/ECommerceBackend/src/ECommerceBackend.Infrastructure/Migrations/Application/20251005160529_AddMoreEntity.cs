using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddMoreEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Products",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                shop_id = table.Column<Guid>(type: "uuid", nullable: false),
                category_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                medias = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_products", x => x.id);
                table.ForeignKey(
                    name: "fk_products_categories_category_id",
                    column: x => x.category_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_products_shop_shop_id",
                    column: x => x.shop_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "shops",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_products_category_id",
            schema: "ecommerce-domain",
            table: "Products",
            column: "category_id");

        migrationBuilder.CreateIndex(
            name: "ix_products_shop_id",
            schema: "ecommerce-domain",
            table: "Products",
            column: "shop_id");

        migrationBuilder.CreateIndex(
            name: "ix_products_slug",
            schema: "ecommerce-domain",
            table: "Products",
            column: "slug",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Products",
            schema: "ecommerce-domain");
    }
}
