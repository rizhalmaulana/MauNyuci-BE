using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class ApplyPostGISLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfiles_Stores_StoreId",
                table: "DriverProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfiles_Users_UserId",
                table: "DriverProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Users_OwnerId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreServices_Stores_StoreId",
                table: "StoreServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreServices",
                table: "StoreServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stores",
                table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverProfiles",
                table: "DriverProfiles");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stores");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "StoreServices",
                newName: "StoreService");

            migrationBuilder.RenameTable(
                name: "Stores",
                newName: "Store");

            migrationBuilder.RenameTable(
                name: "DriverProfiles",
                newName: "DriverProfile");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PhoneNumber",
                table: "User",
                newName: "IX_User_PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "User",
                newName: "IX_User_Email");

            migrationBuilder.RenameIndex(
                name: "IX_StoreServices_StoreId",
                table: "StoreService",
                newName: "IX_StoreService_StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_Stores_OwnerId",
                table: "Store",
                newName: "IX_Store_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfiles_UserId",
                table: "DriverProfile",
                newName: "IX_DriverProfile_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfiles_StoreId",
                table: "DriverProfile",
                newName: "IX_DriverProfile_StoreId");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Store",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreService",
                table: "StoreService",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Store",
                table: "Store",
                column: "Id");

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
                name: "FK_Store_User_OwnerId",
                table: "Store",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreService_Store_StoreId",
                table: "StoreService",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfile_Store_StoreId",
                table: "DriverProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfile_User_UserId",
                table: "DriverProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Store_User_OwnerId",
                table: "Store");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreService_Store_StoreId",
                table: "StoreService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreService",
                table: "StoreService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Store",
                table: "Store");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverProfile",
                table: "DriverProfile");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Store");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "StoreService",
                newName: "StoreServices");

            migrationBuilder.RenameTable(
                name: "Store",
                newName: "Stores");

            migrationBuilder.RenameTable(
                name: "DriverProfile",
                newName: "DriverProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_User_PhoneNumber",
                table: "Users",
                newName: "IX_Users_PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_StoreService_StoreId",
                table: "StoreServices",
                newName: "IX_StoreServices_StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_Store_OwnerId",
                table: "Stores",
                newName: "IX_Stores_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfile_UserId",
                table: "DriverProfiles",
                newName: "IX_DriverProfiles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverProfile_StoreId",
                table: "DriverProfiles",
                newName: "IX_DriverProfiles_StoreId");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stores",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stores",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreServices",
                table: "StoreServices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stores",
                table: "Stores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverProfiles",
                table: "DriverProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverProfiles_Stores_StoreId",
                table: "DriverProfiles",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverProfiles_Users_UserId",
                table: "DriverProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Users_OwnerId",
                table: "Stores",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreServices_Stores_StoreId",
                table: "StoreServices",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
