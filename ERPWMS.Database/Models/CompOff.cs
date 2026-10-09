using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("CompOffs")]
public partial class CompOff
{
    [Key]
    public int CompOffId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public DateOnly EarnedDate { get; set; }
    public DateOnly ExpiryDate { get; set; }

    public decimal Days { get; set; } = 1.0m;

    /// <summary>
    /// Available, Used, Expired
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Available";

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
