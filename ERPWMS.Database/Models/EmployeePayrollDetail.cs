using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class EmployeePayrollDetail
{
    public int PayrollDetailId { get; set; }

    public int PayrollId { get; set; }

    public int EmployeeId { get; set; }

    public int ComponentId { get; set; }

    public decimal Amount { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual SalaryComponent Component { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual EmployeePayroll Payroll { get; set; } = null!;
}
