using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class AddedReviewTableAndUpdatedFormTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "approvedByDeptChair",
                table: "FormTable",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FormTableId = table.Column<int>(type: "int", nullable: false),
                    Area1Criteron = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area1Score = table.Column<float>(type: "real", nullable: false),
                    Area2Criteron = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area2Score = table.Column<float>(type: "real", nullable: false),
                    ProcedureScore = table.Column<float>(type: "real", nullable: false),
                    TimelineScore = table.Column<float>(type: "real", nullable: false),
                    EvaluationScore = table.Column<float>(type: "real", nullable: false),
                    DocumentationScore = table.Column<float>(type: "real", nullable: false),
                    ReviewDone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_FormTable_FormTableId",
                        column: x => x.FormTableId,
                        principalTable: "FormTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FormTableId",
                table: "Reviews",
                column: "FormTableId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropColumn(
                name: "approvedByDeptChair",
                table: "FormTable");
        }
    }
}
