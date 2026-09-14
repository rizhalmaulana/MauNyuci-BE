using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubMenusAndAdjustRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "RequiredRole",
                value: "Store");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"),
                column: "RequiredRole",
                value: "Store");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"),
                column: "RequiredRole",
                value: "Store");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"),
                column: "RequiredRole",
                value: "Store");

            migrationBuilder.InsertData(
                table: "AppMenus",
                columns: new[] { "Id", "AppType", "CreatedAt", "Icon", "IsActive", "ParentId", "Path", "RequiredMembershipTier", "RequiredRole", "SortOrder", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111121"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "local_laundry_service", true, new Guid("11111111-1111-1111-1111-111111111113"), "/layanan/katalog", null, "Store", 1, "Katalog Laundry", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111122"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "inventory", true, new Guid("11111111-1111-1111-1111-111111111113"), "/layanan/stok", null, "Owner", 2, "Katalog Stok", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111131"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "store", true, new Guid("11111111-1111-1111-1111-111111111114"), "/akun/profil", null, "Owner", 1, "Ubah Profil Toko", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111132"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "people", true, new Guid("11111111-1111-1111-1111-111111111114"), "/akun/staff", null, "Owner", 2, "Kelola Staff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111133"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "payment", true, new Guid("11111111-1111-1111-1111-111111111114"), "/akun/pembayaran", null, "Owner", 3, "Metode Pembayaran", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111121"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111122"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111131"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111132"));

            migrationBuilder.DeleteData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111133"));

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "RequiredRole",
                value: "Owner");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"),
                column: "RequiredRole",
                value: "Owner");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"),
                column: "RequiredRole",
                value: "Owner");

            migrationBuilder.UpdateData(
                table: "AppMenus",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"),
                column: "RequiredRole",
                value: "Owner");
        }
    }
}
