using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialMenus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppMenus",
                columns: new[] { "Id", "AppType", "CreatedAt", "Icon", "IsActive", "ParentId", "Path", "RequiredMembershipTier", "RequiredRole", "SortOrder", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Store", new DateTime(2026, 8, 23, 12, 54, 9, 860, DateTimeKind.Utc).AddTicks(7730), "ic_home", true, null, "/home", null, "Store", 1, "Beranda", new DateTime(2026, 8, 23, 12, 54, 9, 860, DateTimeKind.Utc).AddTicks(7736) },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "Store", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(286), "ic_riwayat", true, null, "/pesanan", null, "Store", 2, "Pesanan", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(287) },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "Store", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(301), "local_laundry_service", true, null, "/layanan", null, "Store", 3, "Layanan", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(301) },
                    { new Guid("11111111-1111-1111-1111-111111111114"), "Store", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(306), "ic_akun", true, null, "/akun", null, "Store", 4, "Akun", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(307) },
                    { new Guid("22222222-2222-2222-2222-222222222221"), "Customer", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(324), "ic_home", true, null, "/home", null, "Customer", 1, "Beranda", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(324) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Customer", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(329), "ic_riwayat", true, null, "/history", null, "Customer", 2, "Riwayat", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(330) },
                    { new Guid("22222222-2222-2222-2222-222222222223"), "Customer", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(334), "ic_akun", true, null, "/account", null, "Customer", 3, "Akun", new DateTime(2026, 8, 23, 12, 54, 9, 861, DateTimeKind.Utc).AddTicks(335) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"));
        }
    }
}
