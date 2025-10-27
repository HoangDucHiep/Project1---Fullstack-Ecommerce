using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class ReAddProductEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "products",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                shop_id = table.Column<Guid>(type: "uuid", nullable: false),
                category_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                slug = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
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

        migrationBuilder.CreateTable(
            name: "product_option_types",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                product_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_option_types", x => x.id);
                table.ForeignKey(
                    name: "fk_product_option_types_products_product_id",
                    column: x => x.product_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "product_variants",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                product_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                stock = table.Column<int>(type: "integer", nullable: false),
                weight = table.Column<double>(type: "double precision", nullable: false),
                height = table.Column<double>(type: "double precision", nullable: false),
                width = table.Column<double>(type: "double precision", nullable: false),
                length = table.Column<double>(type: "double precision", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_variants", x => x.id);
                table.ForeignKey(
                    name: "fk_product_variants_products_product_id",
                    column: x => x.product_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "product_option_values",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                product_option_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_option_values", x => x.id);
                table.ForeignKey(
                    name: "fk_product_option_values_product_option_types_product_option_t",
                    column: x => x.product_option_type_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "product_option_types",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "product_medias",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                product_id = table.Column<Guid>(type: "uuid", nullable: false),
                product_variant_id = table.Column<Guid>(type: "uuid", nullable: true),
                media_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_cover = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_medias", x => x.id);
                table.ForeignKey(
                    name: "fk_product_medias_medias_media_id",
                    column: x => x.media_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "medias",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_product_medias_product_variant_product_variant_id",
                    column: x => x.product_variant_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "product_variants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_product_medias_products_product_id",
                    column: x => x.product_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "product_variant_option_values",
            schema: "ecommerce-domain",
            columns: table => new
            {
                variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                option_value_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_variant_option_values", x => new { x.variant_id, x.option_value_id });
                table.ForeignKey(
                    name: "fk_product_variant_option_values_product_option_values_option_",
                    column: x => x.option_value_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "product_option_values",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_product_variant_option_values_product_variants_variant_id",
                    column: x => x.variant_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "product_variants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_media_id",
            schema: "ecommerce-domain",
            table: "product_medias",
            column: "media_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_cover",
            schema: "ecommerce-domain",
            table: "product_medias",
            columns: ["product_id", "is_cover"]);

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_id",
            schema: "ecommerce-domain",
            table: "product_medias",
            column: "product_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_variant_id",
            schema: "ecommerce-domain",
            table: "product_medias",
            column: "product_variant_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_variant_cover",
            schema: "ecommerce-domain",
            table: "product_medias",
            columns: ["product_variant_id", "is_cover"]);

        migrationBuilder.CreateIndex(
            name: "ix_product_option_types_product_id",
            schema: "ecommerce-domain",
            table: "product_option_types",
            column: "product_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_option_types_product_id_name",
            schema: "ecommerce-domain",
            table: "product_option_types",
            columns: ["product_id", "name"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_product_option_values_product_option_type_id",
            schema: "ecommerce-domain",
            table: "product_option_values",
            column: "product_option_type_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_option_values_product_option_type_id_value",
            schema: "ecommerce-domain",
            table: "product_option_values",
            columns: ["product_option_type_id", "value"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_product_variant_option_values_option_value_id",
            schema: "ecommerce-domain",
            table: "product_variant_option_values",
            column: "option_value_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_variant_option_values_variant_id",
            schema: "ecommerce-domain",
            table: "product_variant_option_values",
            column: "variant_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_variants_product_id",
            schema: "ecommerce-domain",
            table: "product_variants",
            column: "product_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_variants_status",
            schema: "ecommerce-domain",
            table: "product_variants",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ix_products_category_id",
            schema: "ecommerce-domain",
            table: "products",
            column: "category_id");

        migrationBuilder.CreateIndex(
            name: "ix_products_description",
            schema: "ecommerce-domain",
            table: "products",
            column: "description");

        migrationBuilder.CreateIndex(
            name: "ix_products_name",
            schema: "ecommerce-domain",
            table: "products",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ix_products_shop_id",
            schema: "ecommerce-domain",
            table: "products",
            column: "shop_id");

        migrationBuilder.CreateIndex(
            name: "ix_products_slug",
            schema: "ecommerce-domain",
            table: "products",
            column: "slug",
            unique: true);

        // Create a unique index on product_variants (sku, shop_id) by joining with products table
        // Sửa function để sử dụng IMMUTABLE thay vì STABLE
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION get_shop_id_from_product(product_uuid uuid)
            RETURNS uuid
            LANGUAGE sql
            IMMUTABLE
            AS $$
                SELECT shop_id FROM ""ecommerce-domain"".products WHERE id = product_uuid;
            $$;
        ");

        migrationBuilder.Sql(@"
            CREATE UNIQUE INDEX IX_ProductVariants_ShopId_Sku 
            ON ""ecommerce-domain"".product_variants (get_shop_id_from_product(product_id), sku);
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "product_medias",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "product_variant_option_values",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "product_option_values",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "product_variants",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "product_option_types",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "products",
            schema: "ecommerce-domain");

        // Drop the unique index on product_variants (sku, shop_id)
        migrationBuilder.Sql("DROP INDEX IF EXISTS IX_ProductVariants_ShopId_Sku;");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_shop_id_from_product(uuid);");
    }
}
