using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValidateIrasId.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadNation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeadNation",
                table: "HarpProjectRecords",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeadNation",
                table: "HarpProjectRecords");
        }
    }
}
