using System;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowDeadlineDto
    {
        public int Id { get; set; }
        public int WorkflowDefinitionId { get; set; }
        public string? WorkflowDefinitionName { get; set; }
        public int StageId { get; set; }
        public string? StageName { get; set; }
        public int DurationHours { get; set; }
        public int? EscalationUserId { get; set; }
        public string? EscalationUserName { get; set; }
        public int? EscalationRoleId { get; set; }
        public string? EscalationRoleName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
