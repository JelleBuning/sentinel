using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentinel.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DotNetUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Systems_Organisations_OrganisationId",
                table: "Systems");

            migrationBuilder.AlterColumn<int>(
                name: "OrganisationId",
                table: "Systems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Systems_Organisations_OrganisationId",
                table: "Systems",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Systems_Organisations_OrganisationId",
                table: "Systems");

            migrationBuilder.AlterColumn<int>(
                name: "OrganisationId",
                table: "Systems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Systems_Organisations_OrganisationId",
                table: "Systems",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");
        }
    }
}
