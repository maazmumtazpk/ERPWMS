using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowTransition
{
    public int TransitionId { get; set; }

    public int WorkflowDefId { get; set; }

    public int StageFromId { get; set; }

    public int StageToId { get; set; }

    public int ActionId { get; set; }

    public bool? IsAllowed { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowAction Action { get; set; } = null!;

    public virtual WorkflowStage StageFrom { get; set; } = null!;

    public virtual WorkflowStage StageTo { get; set; } = null!;

    public virtual WorkflowDefinition WorkflowDef { get; set; } = null!;
}
