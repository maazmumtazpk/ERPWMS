using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowAssignment
{
    public int AssignmentId { get; set; }

    public int WorkflowDefId { get; set; }

    public int StageId { get; set; }

    public int? RoleId { get; set; }

    public int? UserId { get; set; }

    public bool? CanApprove { get; set; }

    public bool? CanReject { get; set; }

    public bool? CanHold { get; set; }

    public bool? CanSendBack { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual AspNetRole? Role { get; set; }

    public virtual WorkflowStage Stage { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual WorkflowDefinition WorkflowDef { get; set; } = null!;
}
