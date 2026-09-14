using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreBankAccountAndOrderRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SelectedStoreBankAccountId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SelectedStoreBankAccountId",
                table: "Orders",
                column: "SelectedStoreBankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_StoreBankAccounts_SelectedStoreBankAccountId",
                table: "Orders",
                column: "SelectedStoreBankAccountId",
                principalTable: "StoreBankAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_StoreBankAccounts_SelectedStoreBankAccountId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SelectedStoreBankAccountId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SelectedStoreBankAccountId",
                table: "Orders");
        }
    }
}
