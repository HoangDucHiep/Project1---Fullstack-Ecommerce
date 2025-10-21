using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddProducts : Migration
{
    private static readonly string[] columns = new[] { "product_option_type_id", "value" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "medias",
            schema: "ecommerce-domain",
            table: "Products");

        migrationBuilder.RenameTable(
            name: "Products",
            schema: "ecommerce-domain",
            newName: "products",
            newSchema: "ecommerce-domain");

        migrationBuilder.AlterColumn<string>(
            name: "status",
            schema: "ecommerce-domain",
            table: "products",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

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
                media_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                sort_order = table.Column<int>(type: "integer", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_medias", x => x.id);
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
            name: "ix_product_medias_product_id",
            schema: "ecommerce-domain",
            table: "product_medias",
            column: "product_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_id_sort_order",
            schema: "ecommerce-domain",
            table: "product_medias",
            columns: ["product_id", "sort_order"]);

        migrationBuilder.CreateIndex(
            name: "ix_product_medias_product_variant_id",
            schema: "ecommerce-domain",
            table: "product_medias",
            column: "product_variant_id");

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
            columns: columns,
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
            name: "ix_product_variants_sku",
            schema: "ecommerce-domain",
            table: "product_variants",
            column: "sku",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_product_variants_status",
            schema: "ecommerce-domain",
            table: "product_variants",
            column: "status");
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

        migrationBuilder.RenameTable(
            name: "products",
            schema: "ecommerce-domain",
            newName: "Products",
            newSchema: "ecommerce-domain");

        migrationBuilder.AlterColumn<int>(
            name: "status",
            schema: "ecommerce-domain",
            table: "Products",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(20)",
            oldMaxLength: 20);

        migrationBuilder.AddColumn<string>(
            name: "medias",
            schema: "ecommerce-domain",
            table: "Products",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: false,
            defaultValue: "");
    }
}
