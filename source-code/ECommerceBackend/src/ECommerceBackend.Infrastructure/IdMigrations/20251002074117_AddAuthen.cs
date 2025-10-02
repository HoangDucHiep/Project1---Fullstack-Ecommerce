using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ECommerceBackend.Infrastructure.IdMigrations;

/// <inheritdoc />
public partial class AddAuthen : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "ecommerce-identity");

        migrationBuilder.CreateTable(
            name: "Roles",
            schema: "ecommerce-identity",
            columns: table => new
            {
                id = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                concurrency_stamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            schema: "ecommerce-identity",
            columns: table => new
            {
                id = table.Column<string>(type: "text", nullable: false),
                phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                is_phone_number_verified = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                password_hash = table.Column<string>(type: "text", nullable: true),
                security_stamp = table.Column<string>(type: "text", nullable: true),
                concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                access_failed_count = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "RoleClaims",
            schema: "ecommerce-identity",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                role_id = table.Column<string>(type: "text", nullable: false),
                claim_type = table.Column<string>(type: "text", nullable: true),
                claim_value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_role_claims", x => x.id);
                table.ForeignKey(
                    name: "fk_role_claims_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "ecommerce-identity",
                    principalTable: "Roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserClaims",
            schema: "ecommerce-identity",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                user_id = table.Column<string>(type: "text", nullable: false),
                claim_type = table.Column<string>(type: "text", nullable: true),
                claim_value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_claims", x => x.id);
                table.ForeignKey(
                    name: "fk_user_claims_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-identity",
                    principalTable: "Users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserLogins",
            schema: "ecommerce-identity",
            columns: table => new
            {
                login_provider = table.Column<string>(type: "text", nullable: false),
                provider_key = table.Column<string>(type: "text", nullable: false),
                provider_display_name = table.Column<string>(type: "text", nullable: true),
                user_id = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                table.ForeignKey(
                    name: "fk_user_logins_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-identity",
                    principalTable: "Users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserRoles",
            schema: "ecommerce-identity",
            columns: table => new
            {
                user_id = table.Column<string>(type: "text", nullable: false),
                role_id = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                table.ForeignKey(
                    name: "fk_user_roles_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "ecommerce-identity",
                    principalTable: "Roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_roles_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-identity",
                    principalTable: "Users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserTokens",
            schema: "ecommerce-identity",
            columns: table => new
            {
                user_id = table.Column<string>(type: "text", nullable: false),
                login_provider = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                table.ForeignKey(
                    name: "fk_user_tokens_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "ecommerce-identity",
                    principalTable: "Users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_role_claims_role_id",
            schema: "ecommerce-identity",
            table: "RoleClaims",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            schema: "ecommerce-identity",
            table: "Roles",
            column: "normalized_name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_claims_user_id",
            schema: "ecommerce-identity",
            table: "UserClaims",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_logins_user_id",
            schema: "ecommerce-identity",
            table: "UserLogins",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_role_id",
            schema: "ecommerce-identity",
            table: "UserRoles",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            schema: "ecommerce-identity",
            table: "Users",
            column: "normalized_email");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            schema: "ecommerce-identity",
            table: "Users",
            column: "normalized_user_name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "RoleClaims",
            schema: "ecommerce-identity");

        migrationBuilder.DropTable(
            name: "UserClaims",
            schema: "ecommerce-identity");

        migrationBuilder.DropTable(
            name: "UserLogins",
            schema: "ecommerce-identity");

        migrationBuilder.DropTable(
            name: "UserRoles",
            schema: "ecommerce-identity");

        migrationBuilder.DropTable(
            name: "UserTokens",
            schema: "ecommerce-identity");

        migrationBuilder.DropTable(
            name: "Roles",
            schema: "ecommerce-identity");

        migrationBuilder.DropTable(
            name: "Users",
            schema: "ecommerce-identity");
    }
}
