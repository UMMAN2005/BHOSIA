using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHOSIA.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AppUserChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Locations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "644dec6f-93aa-4081-a7f6-239518cda189");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Locations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "644dec6f-93aa-4081-a7f6-239518cda189",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
