using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddProductSearchIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Fulltext search index
        migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_products_name_description_gin 
        ON ""ecommerce-domain"".products USING gin(to_tsvector('simple', name || ' ' || description));
    ");

        migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_products_category_status_created 
        ON ""ecommerce-domain"".products (category_id, status, created_at_utc DESC);
    ");

        migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_products_shop_status_created 
        ON ""ecommerce-domain"".products (shop_id, status, created_at_utc DESC);
    ");

        migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_product_variants_price_status 
        ON ""ecommerce-domain"".product_variants (price, status) WHERE status = 'Active';
    ");

        migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_addresses_user_pickup_province 
        ON ""ecommerce-domain"".addresses (user_id, province) WHERE is_pick_up_address = true;
    ");

        migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_addresses_user_pickup_district 
        ON ""ecommerce-domain"".addresses (user_id, district) WHERE is_pick_up_address = true;
    ");

        migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS idx_products_slug 
        ON ""ecommerce-domain"".products (slug) WHERE status = 'Active';
    ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"ecommerce-domain\".idx_products_name_description_gin;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"ecommerce-domain\".idx_products_category_status_created;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"ecommerce-domain\".idx_products_shop_status_created;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"ecommerce-domain\".idx_product_variants_price_status;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"ecommerce-domain\".idx_addresses_user_pickup_province;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"ecommerce-domain\".idx_addresses_user_pickup_district;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"ecommerce-domain\".idx_products_slug;");
    }
}
