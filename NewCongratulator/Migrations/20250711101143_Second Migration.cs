using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewCongratulator.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "firstName",
                table: "HBDdatas");

            migrationBuilder.RenameColumn(
                name: "secondName",
                table: "HBDdatas",
                newName: "fullName");

            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "HBDdatas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "HBDdatas");

            migrationBuilder.RenameColumn(
                name: "fullName",
                table: "HBDdatas",
                newName: "secondName");

            migrationBuilder.AddColumn<string>(
                name: "firstName",
                table: "HBDdatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
