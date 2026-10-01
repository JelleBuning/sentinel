using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mycelium.Api.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddOsNameAndVersionToDeviceInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OsName",
                table: "DeviceInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OsVersion",
                table: "DeviceInformation",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OsName",
                table: "DeviceInformation");

            migrationBuilder.DropColumn(
                name: "OsVersion",
                table: "DeviceInformation");
        }
    }
}
