using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class updatedDatabaseRemovedSalt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "PasswordSalt",
                table: "User",
                newName: "HashedPassword");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HashedPassword",
                table: "User",
                newName: "PasswordSalt");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
