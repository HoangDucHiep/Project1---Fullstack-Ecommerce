using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Application;

/// <inheritdoc />
public partial class AddPermissionAndRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "permissions",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_permissions", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "roles",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                is_system_role = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "user_profiles",
            schema: "ecommerce-domain",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                id_card_full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                id_card_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                id_card_full_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_profiles", x => x.id);
                table.ForeignKey(
                    name: "fk_user_profiles_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "permission_role",
            schema: "ecommerce-domain",
            columns: table => new
            {
                permissions_id = table.Column<Guid>(type: "uuid", nullable: false),
                roles_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_permission_role", x => new { x.permissions_id, x.roles_id });
                table.ForeignKey(
                    name: "fk_permission_role_permission_permissions_id",
                    column: x => x.permissions_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "permissions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_permission_role_role_roles_id",
                    column: x => x.roles_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "role_permissions",
            schema: "ecommerce-domain",
            columns: table => new
            {
                role_id = table.Column<Guid>(type: "uuid", nullable: false),
                permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                permission_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                granted_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                role_id1 = table.Column<Guid>(type: "uuid", nullable: false),
                permission_id1 = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_role_permissions", x => new { x.role_id, x.permission_id });
                table.ForeignKey(
                    name: "fk_role_permissions_permissions_permission_id",
                    column: x => x.permission_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "permissions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_role_permissions_permissions_permission_id1",
                    column: x => x.permission_id1,
                    principalSchema: "ecommerce-domain",
                    principalTable: "permissions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_role_permissions_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_role_permissions_roles_role_id1",
                    column: x => x.role_id1,
                    principalSchema: "ecommerce-domain",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "role_user",
            schema: "ecommerce-domain",
            columns: table => new
            {
                roles_id = table.Column<Guid>(type: "uuid", nullable: false),
                users_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_role_user", x => new { x.roles_id, x.users_id });
                table.ForeignKey(
                    name: "fk_role_user_role_roles_id",
                    column: x => x.roles_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_role_user_user_users_id",
                    column: x => x.users_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_roles",
            schema: "ecommerce-domain",
            columns: table => new
            {
                user_id = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                role_id = table.Column<Guid>(type: "uuid", nullable: false),
                assigned_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                assigned_by = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                user_id1 = table.Column<Guid>(type: "uuid", nullable: false),
                role_id1 = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                table.ForeignKey(
                    name: "fk_user_roles_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_roles_roles_role_id1",
                    column: x => x.role_id1,
                    principalSchema: "ecommerce-domain",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_roles_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-domain",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_roles_users_user_id1",
                    column: x => x.user_id1,
                    principalSchema: "ecommerce-domain",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_permission_role_roles_id",
            schema: "ecommerce-domain",
            table: "permission_role",
            column: "roles_id");

        migrationBuilder.CreateIndex(
            name: "ix_permissions_code",
            schema: "ecommerce-domain",
            table: "permissions",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_permission_code",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "permission_code");

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_permission_id",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "permission_id");

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_permission_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "permission_id1");

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_role_id",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_role_id1",
            schema: "ecommerce-domain",
            table: "role_permissions",
            column: "role_id1");

        migrationBuilder.CreateIndex(
            name: "ix_role_user_users_id",
            schema: "ecommerce-domain",
            table: "role_user",
            column: "users_id");

        migrationBuilder.CreateIndex(
            name: "ix_roles_code",
            schema: "ecommerce-domain",
            table: "roles",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_profiles_user_id",
            schema: "ecommerce-domain",
            table: "user_profiles",
            column: "user_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_role_id",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_role_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "role_id1");

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_user_id",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_user_id1",
            schema: "ecommerce-domain",
            table: "user_roles",
            column: "user_id1");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "permission_role",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "role_permissions",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "role_user",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "user_profiles",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "user_roles",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "permissions",
            schema: "ecommerce-domain");

        migrationBuilder.DropTable(
            name: "roles",
            schema: "ecommerce-domain");
    }
}
