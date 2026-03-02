using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class pramish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "TravelExpenses",
                newName: "RSPGAmount");

            migrationBuilder.RenameColumn(
                name: "TravelExpenseId",
                table: "TravelExpenses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OtherExpensesId",
                table: "OtherExpenses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "EquipmentExpenses",
                newName: "RSPGAmount");

            migrationBuilder.RenameColumn(
                name: "EquipmentExpenseId",
                table: "EquipmentExpenses",
                newName: "Id");

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount1",
                table: "TravelExpenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount2",
                table: "TravelExpenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount1",
                table: "PersonnelExpenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount2",
                table: "PersonnelExpenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount1",
                table: "OtherExpenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount2",
                table: "OtherExpenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount1",
                table: "EquipmentExpenses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAmount2",
                table: "EquipmentExpenses",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherAmount1",
                table: "TravelExpenses");

            migrationBuilder.DropColumn(
                name: "OtherAmount2",
                table: "TravelExpenses");

            migrationBuilder.DropColumn(
                name: "OtherAmount1",
                table: "PersonnelExpenses");

            migrationBuilder.DropColumn(
                name: "OtherAmount2",
                table: "PersonnelExpenses");

            migrationBuilder.DropColumn(
                name: "OtherAmount1",
                table: "OtherExpenses");

            migrationBuilder.DropColumn(
                name: "OtherAmount2",
                table: "OtherExpenses");

            migrationBuilder.DropColumn(
                name: "OtherAmount1",
                table: "EquipmentExpenses");

            migrationBuilder.DropColumn(
                name: "OtherAmount2",
                table: "EquipmentExpenses");

            migrationBuilder.RenameColumn(
                name: "RSPGAmount",
                table: "TravelExpenses",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TravelExpenses",
                newName: "TravelExpenseId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OtherExpenses",
                newName: "OtherExpensesId");

            migrationBuilder.RenameColumn(
                name: "RSPGAmount",
                table: "EquipmentExpenses",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EquipmentExpenses",
                newName: "EquipmentExpenseId");
        }
    }
}
