#pragma warning disable CA1861 // Prefer static readonly fields over constant array arguments
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddIsCoverToProductMedia : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "is_cover",
            schema: "ecommerce-domain",
            table: "product_medias",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_id_is_cover",
            schema: "ecommerce-domain",
            table: "product_medias",
            columns: new[] { "product_id", "is_cover" });

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_variant_id_is_cover",
            schema: "ecommerce-domain",
            table: "product_medias",
            columns: new[] { "product_variant_id", "is_cover" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_product_medias_product_id_is_cover",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.DropIndex(
            name: "ix_product_medias_product_variant_id_is_cover",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.DropColumn(
            name: "is_cover",
            schema: "ecommerce-domain",
            table: "product_medias");
    }
}
