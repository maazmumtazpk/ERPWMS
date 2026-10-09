using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Overtimes")]
public partial class Overtime
{
    [Key]
    public int OvertimeId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateOnly OvertimeDate { get; set; }

    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public decimal ScheduledHours { get; set; } = 0;
    public decimal ActualHours { get; set; } = 0;
    public decimal OTHours { get; set; } = 0;

    public decimal OTRate { get; set; } = 0;
    public decimal OTMultiplier { get; set; } = 1.5m;
    public decimal OTAmount { get; set; } = 0;

    public bool IsHolidayWork { get; set; } = false;
    public bool IsWeeklyOffWork { get; set; } = false;

    /// <summary>
    /// Pending, Approved, Rejected
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Remarks { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

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
}
