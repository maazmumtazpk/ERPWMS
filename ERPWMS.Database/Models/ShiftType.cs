using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("ShiftTypes")]
public partial class ShiftType
{
    [Key]
    public int ShiftTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TypeName { get; set; } = null!;

    /// <summary>
    /// Fixed, Rotating, Night, Flexible, Split
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string TypeCode { get; set; } = "Fixed";

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
