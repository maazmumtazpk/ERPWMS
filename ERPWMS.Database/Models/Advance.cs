using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Advances")]
public partial class Advance
{
    [Key]
    public int AdvanceId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AdvanceNo { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateOnly AdvanceDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public bool IsRecovered { get; set; } = false;
    public DateOnly? RecoveryDate { get; set; }

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
