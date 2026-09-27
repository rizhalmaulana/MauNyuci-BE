using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerLaundryImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerLaundryImageUrl",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerLaundryImageUrl",
                table: "Orders");
        }
    }
}
