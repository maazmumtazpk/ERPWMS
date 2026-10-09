using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class EmployeePayroll
{
    public int PayrollId { get; set; }

    public int SalaryId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly MonthYear { get; set; }

    public decimal GrossPay { get; set; }

    public decimal? TotalDeductions { get; set; }

    public decimal NetPay { get; set; }

    public string? Status { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<EmployeePayrollDetail> EmployeePayrollDetails { get; set; } = new List<EmployeePayrollDetail>();

    public virtual EmployeeSalary1 Salary { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
