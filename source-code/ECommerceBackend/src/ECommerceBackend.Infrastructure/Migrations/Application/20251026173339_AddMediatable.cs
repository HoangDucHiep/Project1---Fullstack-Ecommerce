using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddMediatable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "medias",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                file_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                file_size = table.Column<long>(type: "bigint", nullable: false),
                mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                width = table.Column<int>(type: "integer", nullable: true),
                height = table.Column<int>(type: "integer", nullable: true),
                duration = table.Column<int>(type: "integer", nullable: true),
                uploaded_by = table.Column<Guid>(type: "uuid", nullable: true),
                is_temp = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                confirmed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_medias", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_medias_media_type",
            schema: "ecommerce-domain",
            table: "medias",
            column: "media_type");

        migrationBuilder.CreateIndex(
            name: "ix_medias_temp_cleanup",
            schema: "ecommerce-domain",
            table: "medias",
            columns: ["is_temp", "created_at_utc"]);

        migrationBuilder.CreateIndex(
            name: "ix_medias_uploaded_by",
            schema: "ecommerce-domain",
            table: "medias",
            column: "uploaded_by");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "medias",
            schema: "ecommerce-domain");
    }
}
