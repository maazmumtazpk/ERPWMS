using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowDeadline
{
    public int DeadlineId { get; set; }

    public int WorkflowInstId { get; set; }

    public int StageId { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? EscalatedAt { get; set; }

    public int? EscalatedTo { get; set; }

    public bool? IsMet { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? EscalatedToNavigation { get; set; }

    public virtual WorkflowStage Stage { get; set; } = null!;

    public virtual WorkflowInstance WorkflowInst { get; set; } = null!;
}
