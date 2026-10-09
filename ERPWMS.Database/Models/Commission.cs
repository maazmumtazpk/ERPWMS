using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Commissions")]
public partial class Commission
{
    [Key]
    public int CommissionId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [MaxLength(50)]
    public string CommissionType { get; set; } = "Sales";

    public decimal SalesAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }

    public DateOnly CommissionDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? ReferenceNo { get; set; }

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
