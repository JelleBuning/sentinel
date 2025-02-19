using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentinel.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedDeviceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DeviceDetailsId",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeviceSecurityId",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DeviceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Processor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstalledRam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GraphicsCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDisks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOsDisk = table.Column<bool>(type: "bit", nullable: false),
                    Used = table.Column<double>(type: "float", nullable: false),
                    Size = table.Column<double>(type: "float", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDisks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDisks_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeviceSecurities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastScan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastAntivirusUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAntispywareUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsVirtualmachine = table.Column<bool>(type: "bit", nullable: false),
                    AntivirusEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RealTimeProtectionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    NISEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TamperProtectionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AntispywareEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DomainFirewallEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PrivateFirewallEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PublicFirewallEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSecurities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceSoftware",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSoftware", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceSoftware_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceDetailsId",
                table: "Devices",
                column: "DeviceDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceSecurityId",
                table: "Devices",
                column: "DeviceSecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDisks_DeviceId",
                table: "DeviceDisks",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSoftware_DeviceId",
                table: "DeviceSoftware",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceDetails_DeviceDetailsId",
                table: "Devices",
                column: "DeviceDetailsId",
                principalTable: "DeviceDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceSecurities_DeviceSecurityId",
                table: "Devices",
                column: "DeviceSecurityId",
                principalTable: "DeviceSecurities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceDetails_DeviceDetailsId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceSecurities_DeviceSecurityId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceDetails");

            migrationBuilder.DropTable(
                name: "DeviceDisks");

            migrationBuilder.DropTable(
                name: "DeviceSecurities");

            migrationBuilder.DropTable(
                name: "DeviceSoftware");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceDetailsId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceSecurityId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceDetailsId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceSecurityId",
                table: "Devices");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
