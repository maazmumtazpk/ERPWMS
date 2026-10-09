using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("PayrollRuns")]
public partial class PayrollRun
{
    [Key]
    public int RunId { get; set; }

    [Required]
    [MaxLength(30)]
    public string RunNo { get; set; } = null!;

    [Required]
    public int PeriodId { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }

    public int TotalEmployees { get; set; } = 0;
    public decimal TotalGross { get; set; } = 0;
    public decimal TotalDeductions { get; set; } = 0;
    public decimal TotalNet { get; set; } = 0;

    /// <summary>
    /// Draft, Calculated, Verified, Approved, Posted, Paid, Locked
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Draft";

    public DateTime? CalculatedDate { get; set; }
    public DateTime? VerifiedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? PostedDate { get; set; }
    public DateTime? LockedDate { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    [MaxLength(100)]
    public string? VerifiedBy { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }

    [ForeignKey("PeriodId")]
    public virtual PayrollPeriod Period { get; set; } = null!;

    public virtual ICollection<PayrollRegister> Registers { get; set; } = new List<PayrollRegister>();
}
