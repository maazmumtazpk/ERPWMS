using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("PayrollApprovalHistories")]
public partial class PayrollApprovalHistory
{
    [Key]
    public int HistoryId { get; set; }

    [Required]
    public int RunId { get; set; }

    [MaxLength(50)]
    public string Action { get; set; } = null!;

    [MaxLength(20)]
    public string? FromStatus { get; set; }

    [MaxLength(20)]
    public string? ToStatus { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    [MaxLength(100)]
    public string? ActionBy { get; set; }
    public DateTime? ActionDate { get; set; }

    [ForeignKey("RunId")]
    public virtual PayrollRun Run { get; set; } = null!;
}
