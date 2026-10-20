using System;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowDefinitionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EntityType { get; set; }  // e.g., "PurchaseOrder", "LeaveRequest"
        public bool IsActive { get; set; }
        public int? StartStageId { get; set; }
        public int? FinalStageId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int StageCount { get; set; }
    }
}

