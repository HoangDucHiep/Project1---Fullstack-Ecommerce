using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAddress : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "addresses",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ward = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                address_line = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                is_default = table.Column<bool>(type: "boolean", nullable: false),
                is_pick_up_address = table.Column<bool>(type: "boolean", nullable: false),
                is_return_address = table.Column<bool>(type: "boolean", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_addresses", x => x.id);
                table.ForeignKey(
                    name: "fk_addresses_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_addresses_user_id",
            table: "addresses",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "addresses");
    }
}
