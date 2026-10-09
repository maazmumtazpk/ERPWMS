using System;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowStageDto
    {
        public int Id { get; set; }
        public int WorkflowDefinitionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public bool IsStart { get; set; }
        public bool IsFinal { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? WorkflowDefinitionName { get; set; }
    }
}
