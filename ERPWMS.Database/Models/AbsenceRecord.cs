using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("AbsenceRecords")]
public partial class AbsenceRecord
{
    [Key]
    public int AbsenceId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateOnly AbsenceDate { get; set; }

    [MaxLength(50)]
    public string AbsenceType { get; set; } = "Unpaid"; // Unpaid, Authorized, Unauthorized

    public decimal DeductionDays { get; set; } = 1.0m;

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
