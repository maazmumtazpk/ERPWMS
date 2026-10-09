using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowInstanceLog
{
    public int LogId { get; set; }

    public int WorkflowInstId { get; set; }

    public int? StageFromId { get; set; }

    public int StageToId { get; set; }

    public int? ActionId { get; set; }

    public int UserId { get; set; }

    public string? Comments { get; set; }

    public DateTime? ActionDate { get; set; }

    public string? IPAddress { get; set; }

    public string? UserAgent { get; set; }

    public virtual WorkflowAction? Action { get; set; }

    public virtual WorkflowStage? StageFrom { get; set; }

    public virtual WorkflowStage StageTo { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual WorkflowInstance WorkflowInst { get; set; } = null!;
}
