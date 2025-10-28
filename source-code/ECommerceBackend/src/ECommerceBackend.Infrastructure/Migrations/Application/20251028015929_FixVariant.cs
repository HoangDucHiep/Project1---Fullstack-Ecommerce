using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class FixVariant : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "product_id1",
            schema: "ecommerce-domain",
            table: "product_variants",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "ix_product_variants_product_id1",
            schema: "ecommerce-domain",
            table: "product_variants",
            column: "product_id1");

        migrationBuilder.AddForeignKey(
            name: "fk_product_variants_products_product_id1",
            schema: "ecommerce-domain",
            table: "product_variants",
            column: "product_id1",
            principalSchema: "ecommerce-domain",
            principalTable: "products",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_product_variants_products_product_id1",
            schema: "ecommerce-domain",
            table: "product_variants");

        migrationBuilder.DropIndex(
            name: "ix_product_variants_product_id1",
            schema: "ecommerce-domain",
            table: "product_variants");

        migrationBuilder.DropColumn(
            name: "product_id1",
            schema: "ecommerce-domain",
            table: "product_variants");
    }
}
