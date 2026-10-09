using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("PayrollRegisters")]
public partial class PayrollRegister
{
    [Key]
    public int RegisterId { get; set; }

    [Required]
    public int RunId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    // Earnings
    public decimal BasicSalary { get; set; } = 0;
    public decimal HouseRent { get; set; } = 0;
    public decimal UtilityAllowance { get; set; } = 0;
    public decimal MedicalAllowance { get; set; } = 0;
    public decimal OtherAllowances { get; set; } = 0;
    public decimal GrossSalary { get; set; } = 0;

    // Additional Earnings
    public decimal OvertimeAmount { get; set; } = 0;
    public decimal BonusAmount { get; set; } = 0;
    public decimal CommissionAmount { get; set; } = 0;
    public decimal TotalEarnings { get; set; } = 0;

    // Deductions
    public decimal IncomeTax { get; set; } = 0;
    public decimal ProfessionalTax { get; set; } = 0;
    public decimal LateDeduction { get; set; } = 0;
    public decimal AbsenceDeduction { get; set; } = 0;
    public decimal UnpaidLeaveDeduction { get; set; } = 0;
    public decimal LoanRecovery { get; set; } = 0;
    public decimal AdvanceRecovery { get; set; } = 0;
    public decimal OtherDeductions { get; set; } = 0;
    public decimal TotalDeductions { get; set; } = 0;

    // Net
    public decimal NetPay { get; set; } = 0;

    // Meta
    public int WorkingDays { get; set; } = 0;
    public int PresentDays { get; set; } = 0;
    public int AbsentDays { get; set; } = 0;
    public int LeaveDays { get; set; } = 0;
    public int LateDays { get; set; } = 0;
    public decimal OvertimeHours { get; set; } = 0;

    [MaxLength(500)]
    public string? CalculationNotes { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime? CreatedDate { get; set; }

    [ForeignKey("RunId")]
    public virtual PayrollRun Run { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
