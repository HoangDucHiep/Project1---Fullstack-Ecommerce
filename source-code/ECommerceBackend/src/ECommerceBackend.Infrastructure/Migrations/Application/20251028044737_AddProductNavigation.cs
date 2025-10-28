using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddProductNavigation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "category_id1",
            schema: "ecommerce-domain",
            table: "products",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "product_id1",
            schema: "ecommerce-domain",
            table: "product_medias",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_products_category_id1",
            schema: "ecommerce-domain",
            table: "products",
            column: "category_id1");

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_id1",
            schema: "ecommerce-domain",
            table: "product_medias",
            column: "product_id1");

        migrationBuilder.AddForeignKey(
            name: "fk_product_medias_products_product_id1",
            schema: "ecommerce-domain",
            table: "product_medias",
            column: "product_id1",
            principalSchema: "ecommerce-domain",
            principalTable: "products",
            principalColumn: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_products_categories_category_id1",
            schema: "ecommerce-domain",
            table: "products",
            column: "category_id1",
            principalSchema: "ecommerce-domain",
            principalTable: "categories",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_product_medias_products_product_id1",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.DropForeignKey(
            name: "fk_products_categories_category_id1",
            schema: "ecommerce-domain",
            table: "products");

        migrationBuilder.DropIndex(
            name: "ix_products_category_id1",
            schema: "ecommerce-domain",
            table: "products");

        migrationBuilder.DropIndex(
            name: "ix_product_medias_product_id1",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.DropColumn(
            name: "category_id1",
            schema: "ecommerce-domain",
            table: "products");

        migrationBuilder.DropColumn(
            name: "product_id1",
            schema: "ecommerce-domain",
            table: "product_medias");
    }
}
