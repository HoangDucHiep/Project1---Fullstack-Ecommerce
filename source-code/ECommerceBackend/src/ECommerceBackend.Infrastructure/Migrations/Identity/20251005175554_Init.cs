using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ECommerceBackend.Infrastructure.Migrations.Identity;

/// <inheritdoc />
public partial class Init : Migration
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
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                password_hash = table.Column<string>(type: "text", nullable: true),
                security_stamp = table.Column<string>(type: "text", nullable: true),
                concurrency_stamp = table.Column<string>(type: "text", nullable: true),
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
            name: "RefreshTokens",
            schema: "ecommerce-identity",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                token = table.Column<string>(type: "text", nullable: false),
                jwt_id = table.Column<string>(type: "text", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_used = table.Column<bool>(type: "boolean", nullable: false),
                is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                replaced_by_token = table.Column<string>(type: "text", nullable: true),
                identity_user_id = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_refresh_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_refresh_tokens_users_identity_user_id",
                    column: x => x.identity_user_id,
                    principalSchema: "ecommerce-identity",
                    principalTable: "Users",
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
            name: "ix_refresh_tokens_identity_user_id",
            schema: "ecommerce-identity",
            table: "RefreshTokens",
            column: "identity_user_id");

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
            name: "PhoneNumberIndex",
            schema: "ecommerce-identity",
            table: "Users",
            column: "phone_number");

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
            name: "RefreshTokens",
            schema: "ecommerce-identity");

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
