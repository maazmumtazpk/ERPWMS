using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("PayrollPayments")]
public partial class PayrollPayment
{
    [Key]
    public int PaymentId { get; set; }

    [Required]
    public int RunId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentNo { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }

    /// <summary>
    /// BankTransfer, Cash, Cheque
    /// </summary>
    [MaxLength(20)]
    public string PaymentMethod { get; set; } = "BankTransfer";

    [MaxLength(100)]
    public string? ReferenceNo { get; set; }

    [MaxLength(100)]
    public string? BankName { get; set; }

    [MaxLength(50)]
    public string? ChequeNo { get; set; }

    /// <summary>
    /// Pending, Paid, Failed, Cancelled
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
