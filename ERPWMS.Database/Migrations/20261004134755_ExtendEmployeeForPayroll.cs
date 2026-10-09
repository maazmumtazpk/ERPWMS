using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWMS.Database.Migrations
{
    /// <inheritdoc />
    public partial class ExtendEmployeeForPayroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmploymentType",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LeavePolicyId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OvertimeEligible",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SalaryStructureId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeeklyOffDay",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmploymentType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LeavePolicyId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "OvertimeEligible",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SalaryStructureId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "WeeklyOffDay",
                table: "Employees");
        }
    }
}
