using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class CreateWorkflowDeadlineRequest
    {
        [Required(ErrorMessage = "Workflow Definition is required.")]
        public int WorkflowDefinitionId { get; set; }

        [Required(ErrorMessage = "Stage is required.")]
        public int StageId { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 9999)]
        public int DurationHours { get; set; }

        public int? EscalationUserId { get; set; }
        public int? EscalationRoleId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}