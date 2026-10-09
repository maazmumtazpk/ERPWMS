using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class ExecuteTransitionRequest
    {
        [Required]
        public int WorkflowInstanceId { get; set; }

        [Required]
        public int TransitionId { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }
    }
}