using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Loans")]
public partial class Loan
{
    [Key]
    public int LoanId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string LoanNo { get; set; } = null!;

    public decimal LoanAmount { get; set; }
    public decimal MonthlyRecovery { get; set; }
    public decimal TotalRecovered { get; set; } = 0;
    public decimal RemainingAmount { get; set; }

    public int TotalInstallments { get; set; } = 1;
    public int PaidInstallments { get; set; } = 0;

    public DateOnly IssuedDate { get; set; }
    public DateOnly? StartRecoveryDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

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

    public virtual ICollection<LoanRecovery> Recoveries { get; set; } = new List<LoanRecovery>();
}
