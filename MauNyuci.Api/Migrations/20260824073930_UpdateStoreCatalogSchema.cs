using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreCatalogSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedDurationInHours",
                table: "StoreCatalogItems");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StoreCatalogItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAsset",
                table: "StoreCatalogItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeEstimate",
                table: "StoreCatalogItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "StoreCatalogItems");

            migrationBuilder.DropColumn(
                name: "ImageAsset",
                table: "StoreCatalogItems");

            migrationBuilder.DropColumn(
                name: "TimeEstimate",
                table: "StoreCatalogItems");

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDurationInHours",
                table: "StoreCatalogItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
