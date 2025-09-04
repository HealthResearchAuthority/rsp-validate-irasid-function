using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValidateIrasId.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HarpProjectRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(150)", nullable: false, defaultValueSql: "CAST(NEWID() AS VARCHAR(150))"),
                    IrasId = table.Column<int>(type: "int", nullable: false),
                    RecID = table.Column<int>(type: "int", nullable: true),
                    RecName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortStudyTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FullResearchTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSyncDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarpProjectRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HarpProjectRecords_IrasId",
                table: "HarpProjectRecords",
                column: "IrasId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HarpProjectRecords");
        }
    }
}