using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class UpdateWorkflowTransitionRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Workflow Definition is required.")]
        public int WorkflowDefinitionId { get; set; }

        [Required(ErrorMessage = "Source Stage is required.")]
        public int SourceStageId { get; set; }

        [Required(ErrorMessage = "Destination Stage is required.")]
        public int DestinationStageId { get; set; }

        [Required(ErrorMessage = "Action is required.")]
        public int ActionId { get; set; }

        public bool IsActive { get; set; }
    }
}