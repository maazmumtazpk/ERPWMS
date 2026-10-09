using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("SalaryStructureComponents")]
public partial class SalaryStructureComponent
{
    [Key]
    public int StructureComponentId { get; set; }

    [Required]
    public int StructureId { get; set; }

    [Required]
    public int ComponentId { get; set; }

    public decimal? Value { get; set; }
    public int? BasedOnComponentId { get; set; }
    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("StructureId")]
    public virtual SalaryStructure Structure { get; set; } = null!;

    [ForeignKey("ComponentId")]
    public virtual SalaryComponent Component { get; set; } = null!;
}
