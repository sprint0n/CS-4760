using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class FixedExpenseSetupAndKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormTable_EquipmentExpenses_EquipmentExpenseId",
                table: "FormTable");

            migrationBuilder.DropForeignKey(
                name: "FK_FormTable_OtherExpenses_OtherExpenseId",
                table: "FormTable");

            migrationBuilder.DropForeignKey(
                name: "FK_FormTable_PersonnelExpenses_PersonnelExpenseId",
                table: "FormTable");

            migrationBuilder.DropForeignKey(
                name: "FK_FormTable_TravelExpenses_TravelExpenseId",
                table: "FormTable");

            migrationBuilder.DropIndex(
                name: "IX_FormTable_EquipmentExpenseId",
                table: "FormTable");

            migrationBuilder.DropIndex(
                name: "IX_FormTable_OtherExpenseId",
                table: "FormTable");

            migrationBuilder.DropIndex(
                name: "IX_FormTable_PersonnelExpenseId",
                table: "FormTable");

            migrationBuilder.DropIndex(
                name: "IX_FormTable_TravelExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "EquipmentExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "OtherExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "PersonnelExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "TravelExpenseId",
                table: "FormTable");

            migrationBuilder.RenameColumn(
                name: "ApplicationID",
                table: "TravelExpenses",
                newName: "FormTableId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "PersonnelExpenses",
                newName: "FormTableId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "OtherExpenses",
                newName: "FormTableId");

            migrationBuilder.RenameColumn(
                name: "ApplicationID",
                table: "EquipmentExpenses",
                newName: "FormTableId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelExpenses_FormTableId",
                table: "TravelExpenses",
                column: "FormTableId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelExpenses_FormTableId",
                table: "PersonnelExpenses",
                column: "FormTableId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherExpenses_FormTableId",
                table: "OtherExpenses",
                column: "FormTableId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentExpenses_FormTableId",
                table: "EquipmentExpenses",
                column: "FormTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentExpenses_FormTable_FormTableId",
                table: "EquipmentExpenses",
                column: "FormTableId",
                principalTable: "FormTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherExpenses_FormTable_FormTableId",
                table: "OtherExpenses",
                column: "FormTableId",
                principalTable: "FormTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelExpenses_FormTable_FormTableId",
                table: "PersonnelExpenses",
                column: "FormTableId",
                principalTable: "FormTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TravelExpenses_FormTable_FormTableId",
                table: "TravelExpenses",
                column: "FormTableId",
                principalTable: "FormTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentExpenses_FormTable_FormTableId",
                table: "EquipmentExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherExpenses_FormTable_FormTableId",
                table: "OtherExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelExpenses_FormTable_FormTableId",
                table: "PersonnelExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_TravelExpenses_FormTable_FormTableId",
                table: "TravelExpenses");

            migrationBuilder.DropIndex(
                name: "IX_TravelExpenses_FormTableId",
                table: "TravelExpenses");

            migrationBuilder.DropIndex(
                name: "IX_PersonnelExpenses_FormTableId",
                table: "PersonnelExpenses");

            migrationBuilder.DropIndex(
                name: "IX_OtherExpenses_FormTableId",
                table: "OtherExpenses");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentExpenses_FormTableId",
                table: "EquipmentExpenses");

            migrationBuilder.RenameColumn(
                name: "FormTableId",
                table: "TravelExpenses",
                newName: "ApplicationID");

            migrationBuilder.RenameColumn(
                name: "FormTableId",
                table: "PersonnelExpenses",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "FormTableId",
                table: "OtherExpenses",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "FormTableId",
                table: "EquipmentExpenses",
                newName: "ApplicationID");

            migrationBuilder.AddColumn<int>(
                name: "EquipmentExpenseId",
                table: "FormTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OtherExpenseId",
                table: "FormTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonnelExpenseId",
                table: "FormTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TravelExpenseId",
                table: "FormTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FormTable_EquipmentExpenseId",
                table: "FormTable",
                column: "EquipmentExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTable_OtherExpenseId",
                table: "FormTable",
                column: "OtherExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTable_PersonnelExpenseId",
                table: "FormTable",
                column: "PersonnelExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTable_TravelExpenseId",
                table: "FormTable",
                column: "TravelExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormTable_EquipmentExpenses_EquipmentExpenseId",
                table: "FormTable",
                column: "EquipmentExpenseId",
                principalTable: "EquipmentExpenses",
                principalColumn: "EquipmentExpenseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormTable_OtherExpenses_OtherExpenseId",
                table: "FormTable",
                column: "OtherExpenseId",
                principalTable: "OtherExpenses",
                principalColumn: "OtherExpensesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormTable_PersonnelExpenses_PersonnelExpenseId",
                table: "FormTable",
                column: "PersonnelExpenseId",
                principalTable: "PersonnelExpenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormTable_TravelExpenses_TravelExpenseId",
                table: "FormTable",
                column: "TravelExpenseId",
                principalTable: "TravelExpenses",
                principalColumn: "TravelExpenseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
