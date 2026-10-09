using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("AttendanceCorrectionRequests")]
public partial class AttendanceCorrectionRequest
{
    [Key]
    public int CorrectionId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateOnly AttendanceDate { get; set; }

    public TimeOnly? OriginalInTime { get; set; }
    public TimeOnly? OriginalOutTime { get; set; }

    public TimeOnly? RequestedInTime { get; set; }
    public TimeOnly? RequestedOutTime { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = null!;

    /// <summary>
    /// Pending, Approved, Rejected
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? ApprovalRemarks { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
