using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreEmailColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreEmail",
                table: "Store",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Store_StoreEmail",
                table: "Store",
                column: "StoreEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Store_StorePhoneNumber",
                table: "Store",
                column: "StorePhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Store_StoreEmail",
                table: "Store");

            migrationBuilder.DropIndex(
                name: "IX_Store_StorePhoneNumber",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "StoreEmail",
                table: "Store");
        }
    }
}
