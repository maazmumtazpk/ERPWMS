using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWMS.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayrollSettings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Frequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SalaryDivisor = table.Column<int>(type: "int", nullable: false),
                    WorkingDays = table.Column<int>(type: "int", nullable: false),
                    GracePeriodMinutes = table.Column<int>(type: "int", nullable: false),
                    LateThresholdMinutes = table.Column<int>(type: "int", nullable: false),
                    EarlyThresholdMinutes = table.Column<int>(type: "int", nullable: false),
                    OTMultiplier = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    RoundingRule = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApprovalRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollSettings", x => x.SettingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayrollSettings");
        }
    }
}
