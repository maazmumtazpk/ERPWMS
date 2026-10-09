using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("LeaveBalances")]
public partial class LeaveBalance
{
    [Key]
    public int LeaveBalanceId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int LeaveTypeId { get; set; }

    [Required]
    public int Year { get; set; }

    public decimal Allocated { get; set; } = 0;
    public decimal Used { get; set; } = 0;
    public decimal Pending { get; set; } = 0;
    public decimal CarriedForward { get; set; } = 0;
    public decimal Encashed { get; set; } = 0;

    /// <summary>
    /// Auto-calculated: Allocated + CarriedForward - Used - Pending
    /// </summary>
    public decimal Remaining { get; set; } = 0;

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
