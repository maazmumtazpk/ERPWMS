using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("LoanRecoveries")]
public partial class LoanRecovery
{
    [Key]
    public int RecoveryId { get; set; }

    [Required]
    public int LoanId { get; set; }

    public DateOnly RecoveryDate { get; set; }
    public decimal Amount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsProcessedInPayroll { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("LoanId")]
    public virtual Loan Loan { get; set; } = null!;
}
