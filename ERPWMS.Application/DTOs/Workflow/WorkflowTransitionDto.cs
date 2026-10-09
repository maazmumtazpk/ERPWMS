using System;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowTransitionDto
    {
        public int Id { get; set; }
        public int WorkflowDefinitionId { get; set; }
        public int SourceStageId { get; set; }
        public int DestinationStageId { get; set; }
        public int ActionId { get; set; }
        public string? ActionName { get; set; }
        public string? SourceStageName { get; set; }
        public string? DestinationStageName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? WorkflowDefinitionName { get; set; }
    }
}