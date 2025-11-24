using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValidateIrasId.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustPropertyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortStudyTitle",
                table: "HarpProjectRecords",
                newName: "ShortProjectTitle");

            migrationBuilder.RenameColumn(
                name: "FullResearchTitle",
                table: "HarpProjectRecords",
                newName: "FullProjectTitle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortProjectTitle",
                table: "HarpProjectRecords",
                newName: "ShortStudyTitle");

            migrationBuilder.RenameColumn(
                name: "FullProjectTitle",
                table: "HarpProjectRecords",
                newName: "FullResearchTitle");
        }
    }
}
