using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

/// <summary>
/// Payroll Settings — global configuration for payroll processing (single row table)
/// </summary>
[Table("PayrollSettings")]
public partial class PayrollSetting
{
    [Key]
    public int SettingId { get; set; }

    /// <summary>
    /// Monthly, BiWeekly, Weekly
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Frequency { get; set; } = "Monthly";

    /// <summary>
    /// Days used to calculate daily rate (e.g., 30 or 26)
    /// </summary>
    [Required]
    public int SalaryDivisor { get; set; } = 30;

    /// <summary>
    /// Total working days in a month
    /// </summary>
    [Required]
    public int WorkingDays { get; set; } = 26;

    /// <summary>
    /// Allowed grace period in minutes
    /// </summary>
    public int GracePeriodMinutes { get; set; } = 15;

    /// <summary>
    /// Late threshold in minutes (after this, deduction applies)
    /// </summary>
    public int LateThresholdMinutes { get; set; } = 30;

    /// <summary>
    /// Early departure threshold in minutes
    /// </summary>
    public int EarlyThresholdMinutes { get; set; } = 30;

    /// <summary>
    /// Overtime multiplier (1.5 = 150%, 2.0 = 200%)
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal OTMultiplier { get; set; } = 1.5m;

    /// <summary>
    /// Rounding rule: Nearest, Up, Down, None
    /// </summary>
    [MaxLength(20)]
    public string RoundingRule { get; set; } = "Nearest";

    /// <summary>
    /// Currency code (PKR, USD, etc.)
    /// </summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "PKR";

    /// <summary>
    /// Whether payroll requires approval before posting
    /// </summary>
    public bool ApprovalRequired { get; set; } = true;

    /// <summary>
    /// Active status
    /// </summary>
    public bool IsActive { get; set; } = true;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    // =========================================================
    // STEP 7 ADDITIONS — Late/Early Rules
    // =========================================================

    public int LateWarningCount1 { get; set; } = 3;
    public int LateWarningCount2 { get; set; } = 5;
    public int LateHalfDayCount { get; set; } = 3;
    public int LateFullDayCount { get; set; } = 6;
    public int EarlyWarningCount { get; set; } = 3;
    public int EarlyHalfDayCount { get; set; } = 5;
    public bool EnableLateDeduction { get; set; } = true;
    public bool EnableEarlyDeduction { get; set; } = true;


    /// <summary>Auto-calculate payroll on schedule</summary>
    public bool AutoCalculatePayroll { get; set; } = false;
    /// <summary>Prevent changes after final approval</summary>
    public bool LockAfterApproval { get; set; } = false;
    /// <summary>Allow admins to manually adjust</summary>
    public bool AllowManualAdjustments { get; set; } = true;
    /// <summary>Email notifications to employees</summary>
    public bool SendPayrollNotifications { get; set; } = false;
}

