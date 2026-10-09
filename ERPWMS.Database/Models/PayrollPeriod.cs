using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("PayrollPeriods")]
public partial class PayrollPeriod
{
    [Key]
    public int PeriodId { get; set; }

    [Required]
    [MaxLength(20)]
    public string PeriodCode { get; set; } = null!;

    public int Year { get; set; }
    public int Month { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Open";

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}
