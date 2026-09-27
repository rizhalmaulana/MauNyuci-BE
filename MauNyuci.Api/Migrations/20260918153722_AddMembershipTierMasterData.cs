using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipTierMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembershipTier",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RequiredMembershipTier",
                table: "AppMenus");

            migrationBuilder.AddColumn<Guid>(
                name: "MembershipTierId",
                table: "User",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequiredMembershipTierId",
                table: "AppMenus",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MembershipTiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MonthlyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipTiers", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"),
                column: "RequiredMembershipTierId",
                value: new Guid("11111111-2222-3333-4444-111111111112"));

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111116"),
                column: "RequiredMembershipTierId",
                value: new Guid("11111111-2222-3333-4444-111111111112"));

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111117"),
                column: "RequiredMembershipTierId",
                value: new Guid("11111111-2222-3333-4444-111111111112"));

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111121"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111122"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111131"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111132"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111133"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"),
                column: "RequiredMembershipTierId",
                value: null);

            migrationBuilder.InsertData(
                table: "MembershipTiers",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "MonthlyPrice", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-2222-3333-4444-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Layanan dasar aplikasi", true, 0m, "Regular", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-2222-3333-4444-111111111112"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Akses penuh fitur lanjutan", true, 49000m, "Premium", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_MembershipTierId",
                table: "User",
                column: "MembershipTierId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMenus_RequiredMembershipTierId",
                table: "AppMenus",
                column: "RequiredMembershipTierId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppMenus_MembershipTiers_RequiredMembershipTierId",
                table: "AppMenus",
                column: "RequiredMembershipTierId",
                principalTable: "MembershipTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_User_MembershipTiers_MembershipTierId",
                table: "User",
                column: "MembershipTierId",
                principalTable: "MembershipTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppMenus_MembershipTiers_RequiredMembershipTierId",
                table: "AppMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_User_MembershipTiers_MembershipTierId",
                table: "User");

            migrationBuilder.DropTable(
                name: "MembershipTiers");

            migrationBuilder.DropIndex(
                name: "IX_User_MembershipTierId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_AppMenus_RequiredMembershipTierId",
                table: "AppMenus");

            migrationBuilder.DropColumn(
                name: "MembershipTierId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RequiredMembershipTierId",
                table: "AppMenus");

            migrationBuilder.AddColumn<string>(
                name: "MembershipTier",
                table: "User",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequiredMembershipTier",
                table: "AppMenus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"),
                column: "RequiredMembershipTier",
                value: "Premium");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111116"),
                column: "RequiredMembershipTier",
                value: "Premium");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111117"),
                column: "RequiredMembershipTier",
                value: "Premium");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111121"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111122"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111131"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111132"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111133"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "RequiredMembershipTier",
                value: null);

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"),
                column: "RequiredMembershipTier",
                value: null);
        }
    }
}
