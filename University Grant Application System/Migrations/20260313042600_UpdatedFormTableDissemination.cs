using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedFormTableDissemination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DisseminationBudget",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "DisseminationBudget",
                table: "FormTable",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
