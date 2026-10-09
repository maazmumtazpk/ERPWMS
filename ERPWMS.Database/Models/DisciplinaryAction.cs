using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("DisciplinaryActions")]
public partial class DisciplinaryAction
{
    [Key]
    public int ActionId { get; set; }

    [Required]
    public int IncidentId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Verbal, Written, FinalWarning, Suspension, Termination
    /// </summary>
    [MaxLength(30)]
    public string ActionType { get; set; } = "Verbal";

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    public DateOnly ActionDate { get; set; }
    public int? SuspensionDays { get; set; }
    public decimal? DeductionAmount { get; set; }

    [MaxLength(500)]
    public string? EmployeeExplanation { get; set; }

    [MaxLength(100)]
    public string? TakenBy { get; set; }
    public DateTime? TakenDate { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("IncidentId")]
    public virtual Incident Incident { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
