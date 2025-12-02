using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class FixProductIsDeletedAndCategory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_categories_lft_rgt",
            schema: "ecommerce-domain",
            table: "categories");

        migrationBuilder.DropColumn(
            name: "lft",
            schema: "ecommerce-domain",
            table: "categories");

        migrationBuilder.DropColumn(
            name: "rgt",
            schema: "ecommerce-domain",
            table: "categories");

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            schema: "ecommerce-domain",
            table: "products",
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
            table: "products");

        migrationBuilder.AddColumn<int>(
            name: "lft",
            schema: "ecommerce-domain",
            table: "categories",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "rgt",
            schema: "ecommerce-domain",
            table: "categories",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "ix_categories_lft_rgt",
            schema: "ecommerce-domain",
            table: "categories",
            columns: ["lft", "rgt"]);
    }
}
