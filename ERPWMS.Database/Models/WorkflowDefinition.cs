using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowDefinition
{
    public int WorkflowDefId { get; set; }

    public string WorkflowName { get; set; } = null!;

    public string ModuleType { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<WorkflowAssignment> WorkflowAssignments { get; set; } = new List<WorkflowAssignment>();

    public virtual ICollection<WorkflowCondition> WorkflowConditions { get; set; } = new List<WorkflowCondition>();

    public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();

    public virtual ICollection<WorkflowStage> WorkflowStages { get; set; } = new List<WorkflowStage>();

    public virtual ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
}
