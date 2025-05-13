using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AprovaFacil.Infra.Data.Migrations;

/// <inheritdoc />
public partial class A : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RoleClaims",
            columns: table => new
            {
                Id = table.Column<Int32>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<Int32>(type: "integer", nullable: false),
                ClaimType = table.Column<String>(type: "text", nullable: true),
                ClaimValue = table.Column<String>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoleClaims", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Roles",
            columns: table => new
            {
                Id = table.Column<Int32>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<String>(type: "text", nullable: true),
                NormalizedName = table.Column<String>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<String>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tenants",
            columns: table => new
            {
                Id = table.Column<Int32>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<String>(type: "text", nullable: false),
                Email = table.Column<String>(type: "text", nullable: false),
                PhoneNumber = table.Column<String>(type: "text", nullable: false),
                CNPJ = table.Column<String>(type: "text", nullable: false),
                Address_PostalCode = table.Column<String>(type: "text", nullable: false),
                Address_State = table.Column<String>(type: "text", nullable: false),
                Address_City = table.Column<String>(type: "text", nullable: false),
                Address_Neighborhood = table.Column<String>(type: "text", nullable: false),
                Address_Street = table.Column<String>(type: "text", nullable: false),
                Address_Number = table.Column<String>(type: "text", nullable: false),
                Address_Complement = table.Column<String>(type: "text", nullable: false),
                ContactPerson = table.Column<String>(type: "text", nullable: false),
                Active = table.Column<Boolean>(type: "boolean", nullable: false),
                Plan = table.Column<Int32>(type: "integer", nullable: false),
                MaxRequestsPerMonth = table.Column<Int32>(type: "integer", nullable: false),
                MaxUsers = table.Column<Int32>(type: "integer", nullable: false),
                CurrentRequestsThisMonth = table.Column<Int32>(type: "integer", nullable: false),
                CurrentUserCount = table.Column<Int32>(type: "integer", nullable: false),
                LastRequestResetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                SubscriptionEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserClaims",
            columns: table => new
            {
                Id = table.Column<Int32>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<Int32>(type: "integer", nullable: false),
                ClaimType = table.Column<String>(type: "text", nullable: true),
                ClaimValue = table.Column<String>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserClaims", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<String>(type: "text", nullable: false),
                ProviderKey = table.Column<String>(type: "text", nullable: false),
                ProviderDisplayName = table.Column<String>(type: "text", nullable: true),
                UserId = table.Column<Int32>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
            });

        migrationBuilder.CreateTable(
            name: "UserRoles",
            columns: table => new
            {
                UserId = table.Column<Int32>(type: "integer", nullable: false),
                RoleId = table.Column<Int32>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
            });

        migrationBuilder.CreateTable(
            name: "UserTokens",
            columns: table => new
            {
                UserId = table.Column<Int32>(type: "integer", nullable: false),
                LoginProvider = table.Column<String>(type: "text", nullable: false),
                Name = table.Column<String>(type: "text", nullable: false),
                Value = table.Column<String>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            });

        migrationBuilder.CreateTable(
            name: "Companies",
            columns: table => new
            {
                Id = table.Column<Int32>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CNPJ = table.Column<String>(type: "text", nullable: false),
                TradeName = table.Column<String>(type: "text", nullable: false),
                LegalName = table.Column<String>(type: "text", nullable: false),
                Address_PostalCode = table.Column<String>(type: "text", nullable: false),
                Address_State = table.Column<String>(type: "text", nullable: false),
                Address_City = table.Column<String>(type: "text", nullable: false),
                Address_Neighborhood = table.Column<String>(type: "text", nullable: false),
                Address_Street = table.Column<String>(type: "text", nullable: false),
                Address_Number = table.Column<String>(type: "text", nullable: false),
                Address_Complement = table.Column<String>(type: "text", nullable: false),
                Phone = table.Column<String>(type: "text", nullable: false),
                Email = table.Column<String>(type: "text", nullable: false),
                Enabled = table.Column<Boolean>(type: "boolean", nullable: false),
                TenantId = table.Column<Int32>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Companies", x => x.Id);
                table.ForeignKey(
                    name: "FK_Companies_Tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "Tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Int32>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FullName = table.Column<String>(type: "text", nullable: false),
                Role = table.Column<String>(type: "text", nullable: false),
                Department = table.Column<String>(type: "text", nullable: false),
                PictureUrl = table.Column<String>(type: "text", nullable: false),
                Enabled = table.Column<Boolean>(type: "boolean", nullable: false),
                UserName = table.Column<String>(type: "text", nullable: false),
                Email = table.Column<String>(type: "text", nullable: false),
                SecurityStamp = table.Column<String>(type: "text", nullable: false),
                TenantId = table.Column<Int32>(type: "integer", nullable: false),
                NormalizedUserName = table.Column<String>(type: "text", nullable: true),
                NormalizedEmail = table.Column<String>(type: "text", nullable: true),
                EmailConfirmed = table.Column<Boolean>(type: "boolean", nullable: false),
                PasswordHash = table.Column<String>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<String>(type: "text", nullable: true),
                PhoneNumber = table.Column<String>(type: "text", nullable: true),
                PhoneNumberConfirmed = table.Column<Boolean>(type: "boolean", nullable: false),
                TwoFactorEnabled = table.Column<Boolean>(type: "boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<Boolean>(type: "boolean", nullable: false),
                AccessFailedCount = table.Column<Int32>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    name: "FK_Users_Tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "Tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Requests",
            columns: table => new
            {
                UUID = table.Column<Guid>(type: "uuid", nullable: false),
                CompanyId = table.Column<Int32>(type: "integer", nullable: false),
                RequesterId = table.Column<Int32>(type: "integer", nullable: false),
                FinisherId = table.Column<Int32>(type: "integer", nullable: true),
                InvoiceName = table.Column<Guid>(type: "uuid", nullable: false),
                HasInvoice = table.Column<Boolean>(type: "boolean", nullable: false),
                BudgetName = table.Column<Guid>(type: "uuid", nullable: false),
                HasBudget = table.Column<Boolean>(type: "boolean", nullable: false),
                PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                FirstLevelAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ApprovedFirstLevel = table.Column<Boolean>(type: "boolean", nullable: false),
                SecondLevelAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ApprovedSecondLevel = table.Column<Boolean>(type: "boolean", nullable: false),
                Level = table.Column<Int32>(type: "integer", nullable: false),
                Status = table.Column<Int32>(type: "integer", nullable: false),
                FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Amount = table.Column<Int64>(type: "bigint", nullable: false),
                Note = table.Column<String>(type: "text", nullable: true),
                TenantId = table.Column<Int32>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Requests", x => x.UUID);
                table.ForeignKey(
                    name: "FK_Requests_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Requests_Tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "Tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Requests_Users_FinisherId",
                    column: x => x.FinisherId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Requests_Users_RequesterId",
                    column: x => x.RequesterId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                UUID = table.Column<Guid>(type: "uuid", nullable: false),
                RequestUUID = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Int32>(type: "integer", nullable: false),
                Message = table.Column<String>(type: "text", nullable: false),
                CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Opened = table.Column<Boolean>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.UUID);
                table.ForeignKey(
                    name: "FK_Notifications_Requests_RequestUUID",
                    column: x => x.RequestUUID,
                    principalTable: "Requests",
                    principalColumn: "UUID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Notifications_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RequestDirectors",
            columns: table => new
            {
                RequestUUID = table.Column<Guid>(type: "uuid", nullable: false),
                DirectorId = table.Column<Int32>(type: "integer", nullable: false),
                Approved = table.Column<Int32>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RequestDirectors", x => new { x.RequestUUID, x.DirectorId });
                table.ForeignKey(
                    name: "FK_RequestDirectors_Requests_RequestUUID",
                    column: x => x.RequestUUID,
                    principalTable: "Requests",
                    principalColumn: "UUID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RequestDirectors_Users_DirectorId",
                    column: x => x.DirectorId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RequestManagers",
            columns: table => new
            {
                RequestUUID = table.Column<Guid>(type: "uuid", nullable: false),
                ManagerId = table.Column<Int32>(type: "integer", nullable: false),
                Approved = table.Column<Int32>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RequestManagers", x => new { x.RequestUUID, x.ManagerId });
                table.ForeignKey(
                    name: "FK_RequestManagers_Requests_RequestUUID",
                    column: x => x.RequestUUID,
                    principalTable: "Requests",
                    principalColumn: "UUID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RequestManagers_Users_ManagerId",
                    column: x => x.ManagerId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Companies_TenantId",
            table: "Companies",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_RequestUUID",
            table: "Notifications",
            column: "RequestUUID");

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_UserId",
            table: "Notifications",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_RequestDirectors_DirectorId",
            table: "RequestDirectors",
            column: "DirectorId");

        migrationBuilder.CreateIndex(
            name: "IX_RequestManagers_ManagerId",
            table: "RequestManagers",
            column: "ManagerId");

        migrationBuilder.CreateIndex(
            name: "IX_Requests_CompanyId",
            table: "Requests",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_Requests_FinisherId",
            table: "Requests",
            column: "FinisherId");

        migrationBuilder.CreateIndex(
            name: "IX_Requests_RequesterId",
            table: "Requests",
            column: "RequesterId");

        migrationBuilder.CreateIndex(
            name: "IX_Requests_TenantId",
            table: "Requests",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_TenantId",
            table: "Users",
            column: "TenantId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Notifications");

        migrationBuilder.DropTable(
            name: "RequestDirectors");

        migrationBuilder.DropTable(
            name: "RequestManagers");

        migrationBuilder.DropTable(
            name: "RoleClaims");

        migrationBuilder.DropTable(
            name: "Roles");

        migrationBuilder.DropTable(
            name: "UserClaims");

        migrationBuilder.DropTable(
            name: "UserLogins");

        migrationBuilder.DropTable(
            name: "UserRoles");

        migrationBuilder.DropTable(
            name: "UserTokens");

        migrationBuilder.DropTable(
            name: "Requests");

        migrationBuilder.DropTable(
            name: "Companies");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Tenants");
    }
}
