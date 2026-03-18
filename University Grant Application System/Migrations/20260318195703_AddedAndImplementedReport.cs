using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class AddedAndImplementedReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "UploadedFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormTableId = table.Column<int>(type: "int", nullable: false),
                    newDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    newFundingUse = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_Reports_FormTable_FormTableId",
                        column: x => x.FormTableId,
                        principalTable: "FormTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_ReportId",
                table: "UploadedFiles",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_FormTableId",
                table: "Reports",
                column: "FormTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Reports_ReportId",
                table: "UploadedFiles",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "ReportID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Reports_ReportId",
                table: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_UploadedFiles_ReportId",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "UploadedFiles");
        }
    }
}
