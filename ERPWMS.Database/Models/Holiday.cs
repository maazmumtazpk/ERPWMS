using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Holidays")]
public partial class Holiday
{
    [Key]
    public int HolidayId { get; set; }

    [Required]
    [MaxLength(200)]
    public string HolidayName { get; set; } = null!;

    [Required]
    public DateOnly HolidayDate { get; set; }

    /// <summary>
    /// Public, Religious, National, Company
    /// </summary>
    [MaxLength(50)]
    public string HolidayType { get; set; } = "Public";

    /// <summary>
    /// Company, Branch, Department, All
    /// </summary>
    [MaxLength(50)]
    public string Scope { get; set; } = "All";

    /// <summary>
    /// Agar Scope = Branch
    /// </summary>
    public int? BranchId { get; set; }

    /// <summary>
    /// Agar Scope = Department
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Paid = salary nahi kategi, Unpaid = kategi
    /// </summary>
    public bool IsPaid { get; set; } = true;

    /// <summary>
    /// Kya har saal repeat hoga?
    /// </summary>
    public bool IsRecurring { get; set; } = false;

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

    [MaxLength(100)]
    public string? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}
