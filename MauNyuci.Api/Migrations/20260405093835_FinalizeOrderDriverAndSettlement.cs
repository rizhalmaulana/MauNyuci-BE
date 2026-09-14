using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeOrderDriverAndSettlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfile_Store_StoreId",
                table: "DriverProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfile_User_UserId",
                table: "DriverProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DriverProfile_DriverId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverProfile",
                table: "DriverProfile");

            migrationBuilder.RenameTable(
                name: "DriverProfile",
                newName: "DriverProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfile_UserId",
                table: "DriverProfiles",
                newName: "IX_DriverProfiles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfile_StoreId",
                table: "DriverProfiles",
                newName: "IX_DriverProfiles_StoreId");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryEvidenceUrl",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSettledToStore",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverProfiles",
                table: "DriverProfiles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DriverSettlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SettledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerifiedByAdminId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverSettlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverSettlements_DriverProfiles_DriverId",
                        column: x => x.DriverId,
                        principalTable: "DriverProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverSettlements_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverSettlements_DriverId",
                table: "DriverSettlements",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverSettlements_StoreId",
                table: "DriverSettlements",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverProfiles_Store_StoreId",
                table: "DriverProfiles",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverProfiles_User_UserId",
                table: "DriverProfiles",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DriverProfiles_DriverId",
                table: "Orders",
                column: "DriverId",
                principalTable: "DriverProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfiles_Store_StoreId",
                table: "DriverProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfiles_User_UserId",
                table: "DriverProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DriverProfiles_DriverId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DriverSettlements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverProfiles",
                table: "DriverProfiles");

            migrationBuilder.DropColumn(
                name: "DeliveryEvidenceUrl",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsSettledToStore",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "DriverProfiles",
                newName: "DriverProfile");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfiles_UserId",
                table: "DriverProfile",
                newName: "IX_DriverProfile_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfiles_StoreId",
                table: "DriverProfile",
                newName: "IX_DriverProfile_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverProfile",
                table: "DriverProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverProfile_Store_StoreId",
                table: "DriverProfile",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverProfile_User_UserId",
                table: "DriverProfile",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DriverProfile_DriverId",
                table: "Orders",
                column: "DriverId",
                principalTable: "DriverProfile",
                principalColumn: "Id");
        }
    }
}
