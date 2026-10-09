using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("WeeklyOffs")]
public partial class WeeklyOff
{
    [Key]
    public int WeeklyOffId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, None
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string DayOfWeek { get; set; } = "Sunday";

    /// <summary>
    /// Kab se effective hai
    /// </summary>
    public DateOnly EffectiveFrom { get; set; }

    /// <summary>
    /// Kab tak (null = ongoing)
    /// </summary>
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

    [MaxLength(100)]
    public string? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
