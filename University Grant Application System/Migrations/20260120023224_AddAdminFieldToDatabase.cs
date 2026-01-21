using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminFieldToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAdmin",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAdmin",
                table: "User");
        }
    }
}
