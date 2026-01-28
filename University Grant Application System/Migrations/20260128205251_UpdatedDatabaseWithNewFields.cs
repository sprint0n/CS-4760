using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Grant_Application_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDatabaseWithNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "committeeMemberStatus",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationStatus",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "EquipmentExpense",
                table: "FormTable",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "GrantPurpose",
                table: "FormTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "PersonalExpense",
                table: "FormTable",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "PrincipalInvestigatorID",
                table: "FormTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TotalBudget",
                table: "FormTable",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TravelExpense",
                table: "FormTable",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "isIRB",
                table: "FormTable",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "uploadedFile",
                table: "FormTable",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "DepartmentChairId",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "committeeMemberStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApplicationStatus",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "EquipmentExpense",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "GrantPurpose",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "PersonalExpense",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "PrincipalInvestigatorID",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "TotalBudget",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "TravelExpense",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "isIRB",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "uploadedFile",
                table: "FormTable");

            migrationBuilder.DropColumn(
                name: "DepartmentChairId",
                table: "Departments");
        }
    }
}
