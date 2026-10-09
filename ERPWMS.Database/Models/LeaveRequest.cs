using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("LeaveRequests")]
public partial class LeaveRequest
{
    [Key]
    public int LeaveRequestId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int LeaveTypeId { get; set; }

    [Required]
    public DateOnly FromDate { get; set; }

    [Required]
    public DateOnly ToDate { get; set; }

    public int TotalDays { get; set; }

    public bool IsHalfDay { get; set; } = false;

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = null!;

    /// <summary>
    /// Pending, Approved, Rejected, Cancelled
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? ApprovalRemarks { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Payroll mein adjust ho gaya?
    /// </summary>
    public bool IsProcessedInPayroll { get; set; } = false;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("LeaveTypeId")]
    public virtual LeaveType LeaveType { get; set; } = null!;
}
