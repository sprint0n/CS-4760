using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class AddedExpenseAndFIleUploadTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "uploadedFile",
                table: "FormTable");

            migrationBuilder.RenameColumn(
                name: "TravelExpense",
                table: "FormTable",
                newName: "OtherFunding4Amount");

            migrationBuilder.RenameColumn(
                name: "PersonalExpense",
                table: "FormTable",
                newName: "OtherFunding3Amount");

            migrationBuilder.RenameColumn(
                name: "EquipmentExpense",
                table: "FormTable",
                newName: "OtherFunding2Amount");

            migrationBuilder.AddColumn<float>(
                name: "DisseminationBudget",
                table: "FormTable",
                type: "real",
                nullable: false,
                defaultValue: 0f);

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

            migrationBuilder.AddColumn<float>(
                name: "OtherFunding1Amount",
                table: "FormTable",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "OtherFunding1Name",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OtherFunding2Name",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OtherFunding3Name",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OtherFunding4Name",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PersonnelExpenseId",
                table: "FormTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Timeline",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TravelExpenseId",
                table: "FormTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "pastBudgets",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "pastFunding",
                table: "FormTable",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EquipmentExpenses",
                columns: table => new
                {
                    EquipmentExpenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationID = table.Column<int>(type: "int", nullable: false),
                    EquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentExpenses", x => x.EquipmentExpenseId);
                });

            migrationBuilder.CreateTable(
                name: "OtherExpenses",
                columns: table => new
                {
                    OtherExpensesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    OtherExpenseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherExpenses", x => x.OtherExpensesId);
                });

            migrationBuilder.CreateTable(
                name: "TravelExpenses",
                columns: table => new
                {
                    TravelExpenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationID = table.Column<int>(type: "int", nullable: false),
                    TravelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelExpenses", x => x.TravelExpenseId);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormTableId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_FormTable_FormTableId",
                        column: x => x.FormTableId,
                        principalTable: "FormTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_FormTableId",
                table: "UploadedFiles",
                column: "FormTableId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "EquipmentExpenses");

            migrationBuilder.DropTable(
                name: "OtherExpenses");

            migrationBuilder.DropTable(
                name: "TravelExpenses");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

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
                name: "DisseminationBudget",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "EquipmentExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "OtherExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "OtherFunding1Amount",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "OtherFunding1Name",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "OtherFunding2Name",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "OtherFunding3Name",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "OtherFunding4Name",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "PersonnelExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "TravelExpenseId",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "pastBudgets",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "pastFunding",
                table: "FormTable");

            migrationBuilder.RenameColumn(
                name: "OtherFunding4Amount",
                table: "FormTable",
                newName: "TravelExpense");

            migrationBuilder.RenameColumn(
                name: "OtherFunding3Amount",
                table: "FormTable",
                newName: "PersonalExpense");

            migrationBuilder.RenameColumn(
                name: "OtherFunding2Amount",
                table: "FormTable",
                newName: "EquipmentExpense");

            migrationBuilder.AddColumn<Guid>(
                name: "uploadedFile",
                table: "FormTable",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
