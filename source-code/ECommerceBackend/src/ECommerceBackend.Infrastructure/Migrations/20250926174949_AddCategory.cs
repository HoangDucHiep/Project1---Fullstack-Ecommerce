using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddCategory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "categories",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                lft = table.Column<int>(type: "integer", nullable: false),
                rgt = table.Column<int>(type: "integer", nullable: false),
                depth = table.Column<int>(type: "integer", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_categories", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_categories_lft_rgt",
            table: "categories",
            columns: ["lft", "rgt"]);

        migrationBuilder.CreateIndex(
            name: "ix_categories_name",
            table: "categories",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ix_categories_parent_id",
            table: "categories",
            column: "parent_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "categories");
    }
}
