using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TmsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchieved",
                table: "Enrollments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchieved",
                table: "Enrollments");
        }
    }
}
