using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SalaryComponent
{
    public int ComponentId { get; set; }

    public string ComponentName { get; set; } = null!;

    public string ComponentCode { get; set; } = null!;

    public string ComponentType { get; set; } = null!;

    // STEP 17 ADDITIONS
    public string CalculationType { get; set; } = "Fixed";
    public decimal? Amount { get; set; }
    public decimal? Percentage { get; set; }
    public bool IsTaxable { get; set; } = true;
    public bool IsEarning { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public bool ShowInPayslip { get; set; } = true;

    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<EmployeePayrollDetail> EmployeePayrollDetails { get; set; } = new List<EmployeePayrollDetail>();
}

