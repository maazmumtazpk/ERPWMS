using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Bonuses")]
public partial class Bonus
{
    [Key]
    public int BonusId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [MaxLength(50)]
    public string BonusType { get; set; } = "Performance";

    public decimal Amount { get; set; }
    public decimal? Percentage { get; set; }

    public DateOnly BonusDate { get; set; }
    public DateOnly? PaymentDate { get; set; }

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

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
