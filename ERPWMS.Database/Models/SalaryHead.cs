using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPWMS.Database.Models;

[Table("SalaryHeads")]
public partial class SalaryHead
{
    [Key]
    public int HeadId { get; set; }

    [Required]
    [MaxLength(20)]
    public string HeadCode { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string HeadName { get; set; } = null!;

    [MaxLength(20)]
    public string HeadType { get; set; } = "Earning";

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}
