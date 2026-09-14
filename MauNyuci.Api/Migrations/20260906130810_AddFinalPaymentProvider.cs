using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalPaymentProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinalPaymentProvider",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalPaymentProvider",
                table: "Orders");
        }
    }
}
