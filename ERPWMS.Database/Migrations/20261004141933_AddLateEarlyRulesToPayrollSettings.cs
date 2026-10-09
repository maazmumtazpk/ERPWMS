using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWMS.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLateEarlyRulesToPayrollSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EarlyHalfDayCount",
                table: "PayrollSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EarlyWarningCount",
                table: "PayrollSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableEarlyDeduction",
                table: "PayrollSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableLateDeduction",
                table: "PayrollSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LateFullDayCount",
                table: "PayrollSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LateHalfDayCount",
                table: "PayrollSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LateWarningCount1",
                table: "PayrollSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LateWarningCount2",
                table: "PayrollSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarlyHalfDayCount",
                table: "PayrollSettings");

            migrationBuilder.DropColumn(
                name: "EarlyWarningCount",
                table: "PayrollSettings");

            migrationBuilder.DropColumn(
                name: "EnableEarlyDeduction",
                table: "PayrollSettings");

            migrationBuilder.DropColumn(
                name: "EnableLateDeduction",
                table: "PayrollSettings");

            migrationBuilder.DropColumn(
                name: "LateFullDayCount",
                table: "PayrollSettings");

            migrationBuilder.DropColumn(
                name: "LateHalfDayCount",
                table: "PayrollSettings");

            migrationBuilder.DropColumn(
                name: "LateWarningCount1",
                table: "PayrollSettings");

            migrationBuilder.DropColumn(
                name: "LateWarningCount2",
                table: "PayrollSettings");
        }
    }
}
