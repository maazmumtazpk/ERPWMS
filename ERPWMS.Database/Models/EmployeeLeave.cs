using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class EmployeeLeave
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public string? LeaveType { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public int TotalDays { get; set; }

    public string Reason { get; set; } = null!;

    public string? Status { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? RejectionReason { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
