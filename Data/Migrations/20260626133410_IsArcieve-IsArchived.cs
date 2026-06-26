using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TmsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsArcieveIsArchived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsArchieved",
                table: "Enrollments",
                newName: "IsArchived");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "Enrollments",
                newName: "IsArchieved");
        }
    }
}
