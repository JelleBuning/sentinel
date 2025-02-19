using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentinel.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDeviceTypeFromDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
