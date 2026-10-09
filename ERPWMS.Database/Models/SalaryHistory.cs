using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("SalaryHistories")]
public partial class SalaryHistory
{
    [Key]
    public int HistoryId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public decimal OldBasicSalary { get; set; }
    public decimal NewBasicSalary { get; set; }
    public decimal IncrementAmount { get; set; }
    public decimal IncrementPercent { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

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
