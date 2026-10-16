using System;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowConditionDto
    {
        public int Id { get; set; }
        public int WorkflowDefinitionId { get; set; }
        public int? TransitionId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? WorkflowDefinitionName { get; set; }
        public string? TransitionName { get; set; }
    }
}
