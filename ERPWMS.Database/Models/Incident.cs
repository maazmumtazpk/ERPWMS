using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("Incidents")]
public partial class Incident
{
    [Key]
    public int IncidentId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateOnly IncidentDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string IncidentType { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    [MaxLength(50)]
    public string Severity { get; set; } = "Low";

    [MaxLength(200)]
    public string? Witnesses { get; set; }

    /// <summary>
    /// Open, Investigating, ActionTaken, Closed
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Open";

    [MaxLength(100)]
    public string? ReportedBy { get; set; }
    public DateTime? ReportedDate { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
