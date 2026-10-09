using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowCondition
{
    public int ConditionId { get; set; }

    public int WorkflowDefId { get; set; }

    public int StageFromId { get; set; }

    public int StageToId { get; set; }

    public string ConditionType { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? ActionOnFail { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowStage StageFrom { get; set; } = null!;

    public virtual WorkflowStage StageTo { get; set; } = null!;

    public virtual WorkflowDefinition WorkflowDef { get; set; } = null!;
}
