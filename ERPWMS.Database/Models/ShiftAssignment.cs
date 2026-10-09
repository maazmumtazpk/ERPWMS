using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("ShiftAssignments")]
public partial class ShiftAssignment
{
    [Key]
    public int ShiftAssignmentId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int ShiftId { get; set; }

    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }

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

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("ShiftId")]
    public virtual Shift Shift { get; set; } = null!;
}
