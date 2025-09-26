using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations;

/// <inheritdoc />
public partial class FixTableNaming : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "pk_user",
            table: "user");

        migrationBuilder.RenameTable(
            name: "user",
            newName: "users");

        migrationBuilder.RenameIndex(
            name: "ix_user_identity_id",
            table: "users",
            newName: "ix_users_identity_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_users",
            table: "users",
            column: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "pk_users",
            table: "users");

        migrationBuilder.RenameTable(
            name: "users",
            newName: "user");

        migrationBuilder.RenameIndex(
            name: "ix_users_identity_id",
            table: "user",
            newName: "ix_user_identity_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_user",
            table: "user",
            column: "id");
    }
}
