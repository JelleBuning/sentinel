using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentinel.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVirtualmachine",
                table: "DeviceSecurities",
                newName: "IsVirtualMachine");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DeviceSoftware",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVirtualMachine",
                table: "DeviceSecurities",
                newName: "IsVirtualmachine");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DeviceSoftware",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
