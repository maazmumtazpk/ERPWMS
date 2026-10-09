using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Payslips")]
public partial class Payslip
{
    [Key]
    public int PayslipId { get; set; }

    [Required]
    [MaxLength(50)]
    public string PayslipNo { get; set; } = null!;

    [Required]
    public int RegisterId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }

    public decimal GrossPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }

    public DateTime? GeneratedDate { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("RegisterId")]
    public virtual PayrollRegister Register { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
