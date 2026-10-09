using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("SalaryStructures")]
public partial class SalaryStructure
{
    [Key]
    public int StructureId { get; set; }

    [Required]
    [MaxLength(20)]
    public string StructureCode { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string StructureName { get; set; } = null!;

    [MaxLength(50)]
    public string Grade { get; set; } = "Worker";

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<SalaryStructureComponent> Components { get; set; } = new List<SalaryStructureComponent>();
}
