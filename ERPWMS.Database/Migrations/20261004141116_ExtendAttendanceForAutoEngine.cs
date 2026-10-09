using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWMS.Database.Migrations
{
    /// <inheritdoc />
    public partial class ExtendAttendanceForAutoEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EarlyMinutes",
                table: "EmployeeAttendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHalfDay",
                table: "EmployeeAttendance",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHolidayWork",
                table: "EmployeeAttendance",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWeeklyOffWork",
                table: "EmployeeAttendance",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LateMinutes",
                table: "EmployeeAttendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OTHours",
                table: "EmployeeAttendance",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "EmployeeAttendance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingMinutes",
                table: "EmployeeAttendance",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarlyMinutes",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "IsHalfDay",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "IsHolidayWork",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "IsWeeklyOffWork",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "LateMinutes",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "OTHours",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "WorkingMinutes",
                table: "EmployeeAttendance");
        }
    }
}
