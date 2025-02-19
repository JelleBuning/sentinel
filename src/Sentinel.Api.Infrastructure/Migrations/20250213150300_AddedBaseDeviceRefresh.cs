using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentinel.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBaseDeviceRefresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Devices");
        }
    }
}
