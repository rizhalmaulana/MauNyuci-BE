using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderDriverAndLogisticsFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DriverProfiles_DriverId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Orders",
                newName: "PickupDriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DriverId",
                table: "Orders",
                newName: "IX_Orders_PickupDriverId");

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryDriverId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryTimeSlot",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogisticsNotes",
                table: "Orders",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OTPCode",
                table: "Orders",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupTimeSlot",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryDriverId",
                table: "Orders",
                column: "DeliveryDriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DriverProfiles_DeliveryDriverId",
                table: "Orders",
                column: "DeliveryDriverId",
                principalTable: "DriverProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DriverProfiles_PickupDriverId",
                table: "Orders",
                column: "PickupDriverId",
                principalTable: "DriverProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DriverProfiles_DeliveryDriverId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DriverProfiles_PickupDriverId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryDriverId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryDriverId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryTimeSlot",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LogisticsNotes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OTPCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupTimeSlot",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PickupDriverId",
                table: "Orders",
                newName: "DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_PickupDriverId",
                table: "Orders",
                newName: "IX_Orders_DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DriverProfiles_DriverId",
                table: "Orders",
                column: "DriverId",
                principalTable: "DriverProfiles",
                principalColumn: "Id");
        }
    }
}
