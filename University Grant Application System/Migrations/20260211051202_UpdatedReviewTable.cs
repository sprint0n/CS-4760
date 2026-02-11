using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area1Criteron",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Area1Score",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Area2Criteron",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Area2Score",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "DocumentationScore",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "EvaluationScore",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProcedureScore",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "TimelineScore",
                table: "Reviews",
                newName: "totalScore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "totalScore",
                table: "Reviews",
                newName: "TimelineScore");

            migrationBuilder.AddColumn<string>(
                name: "Area1Criteron",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Area1Score",
                table: "Reviews",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Area2Criteron",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Area2Score",
                table: "Reviews",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DocumentationScore",
                table: "Reviews",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "EvaluationScore",
                table: "Reviews",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ProcedureScore",
                table: "Reviews",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
