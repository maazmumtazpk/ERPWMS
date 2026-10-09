using System;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowInstanceDto
    {
        public int Id { get; set; }
        public int WorkflowDefinitionId { get; set; }
        public string? WorkflowDefinitionName { get; set; }
        public int CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public int? EntityId { get; set; } // Reference to the business record (e.g., PurchaseOrderId)
        public string? EntityType { get; set; }
        public string Status { get; set; } = "Active"; // Active, Completed, Rejected, Cancelled
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}