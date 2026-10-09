using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWMS.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddGroup4SalaryStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "SalaryComponents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalculationType",
                table: "SalaryComponents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SalaryComponents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "SalaryComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsEarning",
                table: "SalaryComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxable",
                table: "SalaryComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentage",
                table: "SalaryComponents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInPayslip",
                table: "SalaryComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Advances",
                columns: table => new
                {
                    AdvanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AdvanceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdvanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsRecovered = table.Column<bool>(type: "bit", nullable: false),
                    RecoveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advances", x => x.AdvanceId);
                    table.ForeignKey(
                        name: "FK_Advances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bonuses",
                columns: table => new
                {
                    BonusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BonusType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BonusDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsProcessedInPayroll = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bonuses", x => x.BonusId);
                    table.ForeignKey(
                        name: "FK_Bonuses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    CommissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CommissionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsProcessedInPayroll = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.CommissionId);
                    table.ForeignKey(
                        name: "FK_Commissions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LoanNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyRecovery = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRecovered = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInstallments = table.Column<int>(type: "int", nullable: false),
                    PaidInstallments = table.Column<int>(type: "int", nullable: false),
                    IssuedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartRecoveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                    table.ForeignKey(
                        name: "FK_Loans_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryHeads",
                columns: table => new
                {
                    HeadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeadCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HeadName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeadType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryHeads", x => x.HeadId);
                });

            migrationBuilder.CreateTable(
                name: "SalaryHistories",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OldBasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewBasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncrementAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncrementPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryHistories", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_SalaryHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructures",
                columns: table => new
                {
                    StructureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StructureCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StructureName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryStructures", x => x.StructureId);
                });

            migrationBuilder.CreateTable(
                name: "LoanRecoveries",
                columns: table => new
                {
                    RecoveryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    RecoveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsProcessedInPayroll = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRecoveries", x => x.RecoveryId);
                    table.ForeignKey(
                        name: "FK_LoanRecoveries_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructureComponents",
                columns: table => new
                {
                    StructureComponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StructureId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BasedOnComponentId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryStructureComponents", x => x.StructureComponentId);
                    table.ForeignKey(
                        name: "FK_SalaryStructureComponents_SalaryComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "SalaryComponents",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalaryStructureComponents_SalaryStructures_StructureId",
                        column: x => x.StructureId,
                        principalTable: "SalaryStructures",
                        principalColumn: "StructureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advances_EmployeeId",
                table: "Advances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bonuses_EmployeeId",
                table: "Bonuses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_EmployeeId",
                table: "Commissions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRecoveries_LoanId",
                table: "LoanRecoveries",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_EmployeeId",
                table: "Loans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryHistories_EmployeeId",
                table: "SalaryHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructureComponents_ComponentId",
                table: "SalaryStructureComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructureComponents_StructureId",
                table: "SalaryStructureComponents",
                column: "StructureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advances");

            migrationBuilder.DropTable(
                name: "Bonuses");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "LoanRecoveries");

            migrationBuilder.DropTable(
                name: "SalaryHeads");

            migrationBuilder.DropTable(
                name: "SalaryHistories");

            migrationBuilder.DropTable(
                name: "SalaryStructureComponents");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "SalaryStructures");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "CalculationType",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "IsEarning",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "IsTaxable",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "ShowInPayslip",
                table: "SalaryComponents");
        }
    }
}
