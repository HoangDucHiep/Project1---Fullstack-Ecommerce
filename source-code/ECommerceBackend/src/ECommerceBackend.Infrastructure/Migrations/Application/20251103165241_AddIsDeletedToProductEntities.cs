using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddIsDeletedToProductEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_variants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_variant_option_values",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_option_values",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_option_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            schema: "ecommerce-domain",
            table: "product_medias",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_variants");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_variant_option_values");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_option_values");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "ecommerce-domain",
                table: "product_option_types");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            schema: "ecommerce-domain",
            table: "product_medias");
    }
}
