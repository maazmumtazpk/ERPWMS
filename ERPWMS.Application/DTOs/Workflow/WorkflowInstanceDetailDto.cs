using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowInstanceDetailDto : WorkflowInstanceDto
    {
        public List<WorkflowInstanceLogDto> Logs { get; set; } = new();
        public List<AvailableActionDto> AvailableActions { get; set; } = new();
        public List<WorkflowAssignmentDto> Assignments { get; set; } = new();
    }

    public class AvailableActionDto
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public int TransitionId { get; set; }
        public int DestinationStageId { get; set; }
        public string DestinationStageName { get; set; } = string.Empty;
    }
}