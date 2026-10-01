using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mycelium.Api.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RenameDeviceDetailsAndAddDiskHealth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceDetails_DeviceDetailsId",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceDetails",
                table: "DeviceDetails");

            migrationBuilder.RenameTable(
                name: "DeviceDetails",
                newName: "DeviceInformation");

            migrationBuilder.RenameColumn(
                name: "DeviceDetailsId",
                table: "Devices",
                newName: "DeviceInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_Devices_DeviceDetailsId",
                table: "Devices",
                newName: "IX_Devices_DeviceInformationId");

            migrationBuilder.AddColumn<string>(
                name: "HealthStatus",
                table: "DeviceDisks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceInformation",
                table: "DeviceInformation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceInformation_DeviceInformationId",
                table: "Devices",
                column: "DeviceInformationId",
                principalTable: "DeviceInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceInformation_DeviceInformationId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HealthStatus",
                table: "DeviceDisks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceInformation",
                table: "DeviceInformation");

            migrationBuilder.RenameTable(
                name: "DeviceInformation",
                newName: "DeviceDetails");

            migrationBuilder.RenameColumn(
                name: "DeviceInformationId",
                table: "Devices",
                newName: "DeviceDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_Devices_DeviceInformationId",
                table: "Devices",
                newName: "IX_Devices_DeviceDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceDetails",
                table: "DeviceDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceDetails_DeviceDetailsId",
                table: "Devices",
                column: "DeviceDetailsId",
                principalTable: "DeviceDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
