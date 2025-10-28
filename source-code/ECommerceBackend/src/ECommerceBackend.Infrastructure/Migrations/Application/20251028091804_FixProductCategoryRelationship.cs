using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class FixProductCategoryRelationship : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_products_categories_category_id1",
            schema: "ecommerce-domain",
            table: "products");

        migrationBuilder.DropIndex(
            name: "ix_products_category_id1",
            schema: "ecommerce-domain",
            table: "products");

        migrationBuilder.DropColumn(
            name: "category_id1",
            schema: "ecommerce-domain",
            table: "products");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "category_id1",
            schema: "ecommerce-domain",
            table: "products",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "ix_products_category_id1",
            schema: "ecommerce-domain",
            table: "products",
            column: "category_id1");

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
}
