using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class FixForeignKeyRelationships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_product_medias_products_product_id1",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.DropForeignKey(
            name: "fk_role_permissions_permissions_permission_id1",
            schema: "ecommerce-domain",
            table: "role_permissions");

        migrationBuilder.DropForeignKey(
            name: "fk_role_permissions_roles_role_id1",
            schema: "ecommerce-domain",
            table: "role_permissions");

        migrationBuilder.DropForeignKey(
            name: "fk_user_roles_roles_role_id1",
            schema: "ecommerce-domain",
            table: "user_roles");

        migrationBuilder.DropForeignKey(
            name: "fk_user_roles_users_user_id1",
            schema: "ecommerce-domain",
            table: "user_roles");

        migrationBuilder.DropIndex(
            name: "ix_user_roles_role_id1",
            schema: "ecommerce-domain",
            table: "user_roles");

        migrationBuilder.DropIndex(
            name: "ix_user_roles_user_id1",
            schema: "ecommerce-domain",
            table: "user_roles");

        migrationBuilder.DropIndex(
            name: "ix_role_permissions_permission_id1",
            schema: "ecommerce-domain",
            table: "role_permissions");

        migrationBuilder.DropIndex(
            name: "ix_role_permissions_role_id1",
            schema: "ecommerce-domain",
            table: "role_permissions");

        migrationBuilder.DropIndex(
            name: "ix_product_medias_product_id1",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.DropColumn(
            name: "role_id1",
            schema: "ecommerce-domain",
            table: "user_roles");

        migrationBuilder.DropColumn(
            name: "user_id1",
            schema: "ecommerce-domain",
            table: "user_roles");

        migrationBuilder.DropColumn(
            name: "permission_id1",
            schema: "ecommerce-domain",
            table: "role_permissions");

        migrationBuilder.DropColumn(
            name: "role_id1",
            schema: "ecommerce-domain",
            table: "role_permissions");

        migrationBuilder.DropColumn(
            name: "product_id1",
            schema: "ecommerce-domain",
            table: "product_medias");

        migrationBuilder.AddColumn<Guid>(
            name: "product_id1",
            schema: "ecommerce-domain",
            table: "product_variants",
            type: "uuid",
            nullable: true);

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
            principalColumn: "id");
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

        migrationBuilder.AddColumn<Guid>(
            name: "role_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "user_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "permission_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "role_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
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
            name: "ix_user_roles_role_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "role_id1");

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_user_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "user_id1");

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_permission_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "permission_id1");

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_role_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "role_id1");

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
            name: "fk_role_permissions_permissions_permission_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "permission_id1",
            principalSchema: "ecommerce-domain",
            principalTable: "permissions",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_role_permissions_roles_role_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "role_id1",
            principalSchema: "ecommerce-domain",
            principalTable: "roles",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_roles_roles_role_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "role_id1",
            principalSchema: "ecommerce-domain",
            principalTable: "roles",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_roles_users_user_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "user_id1",
            principalSchema: "ecommerce-domain",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
