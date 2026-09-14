using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreMenuRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
