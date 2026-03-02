using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class DropdownDatabaseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "grantTypeSelection",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "staffTypeSelection",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grantTypeSelection",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "staffTypeSelection",
                table: "FormTable");
        }
    }
}
