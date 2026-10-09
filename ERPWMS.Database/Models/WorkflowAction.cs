using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowAction
{
    public int ActionId { get; set; }

    public string ActionName { get; set; } = null!;

    public string ActionCode { get; set; } = null!;

    public bool? IsFinal { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<WorkflowInstanceLog> WorkflowInstanceLogs { get; set; } = new List<WorkflowInstanceLog>();

    public virtual ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
}
