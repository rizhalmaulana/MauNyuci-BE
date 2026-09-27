using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationDataPayload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataPayload",
                table: "Notifications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPayload",
                table: "Notifications");
        }
    }
}
