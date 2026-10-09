using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("LeaveTypes")]
public partial class LeaveType
{
    [Key]
    public int LeaveTypeId { get; set; }

    [Required]
    [MaxLength(20)]
    public string LeaveCode { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LeaveName { get; set; } = null!;

    /// <summary>
    /// Annual, Sick, Casual, Maternity, Unpaid, Emergency
    /// </summary>
    [MaxLength(50)]
    public string Category { get; set; } = "Annual";

    /// <summary>
    /// Saal mein kitne din milte hain
    /// </summary>
    public int AnnualEntitlement { get; set; } = 0;

    /// <summary>
    /// Paid ya Unpaid
    /// </summary>
    public bool IsPaid { get; set; } = true;

    /// <summary>
    /// Aagle saal mein carry forward hoga?
    /// </summary>
    public bool AllowCarryForward { get; set; } = false;

    /// <summary>
    /// Maximum carry forward days
    /// </summary>
    public int MaxCarryForwardDays { get; set; } = 0;

    /// <summary>
    /// Maximum consecutive days
    /// </summary>
    public int MaxConsecutiveDays { get; set; } = 30;

    /// <summary>
    /// Approval chahiye?
    /// </summary>
    public bool ApprovalRequired { get; set; } = true;

    /// <summary>
    /// Documents chahiye (jaise maternity ke liye)?
    /// </summary>
    public bool DocumentRequired { get; set; } = false;

    /// <summary>
    /// Notice days (kitne din pehle apply karna)
    /// </summary>
    public int NoticeDays { get; set; } = 0;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
