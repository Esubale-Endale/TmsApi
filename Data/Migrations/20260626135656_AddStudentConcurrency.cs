using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TmsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Students",
                newName: "IsActived");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Students",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "IsActived",
                table: "Students",
                newName: "IsActive");
        }
    }
}
