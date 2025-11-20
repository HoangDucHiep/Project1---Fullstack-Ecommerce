using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class FixProductMedia : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "sort_order",
            schema: "ecommerce-domain",
            table: "product_medias",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            comment: "Video: -1, Images: >= 0",
            oldClrType: typeof(int),
            oldType: "integer",
            oldDefaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_sort_order",
            schema: "ecommerce-domain",
            table: "product_medias",
            columns: ["product_id", "sort_order"]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_product_medias_product_sort_order",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.AlterColumn<int>(
            name: "sort_order",
            schema: "ecommerce-domain",
            table: "product_medias",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldDefaultValue: 0,
            oldComment: "Video: -1, Images: >= 0");
    }
}
