using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class UpdateWorkflowStageRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Workflow Definition is required.")]
        public int WorkflowDefinitionId { get; set; }

        [Required(ErrorMessage = "Stage name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stage code is required.")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Code can only contain letters, numbers, and underscores.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 999)]
        public int Order { get; set; }

        public bool IsStart { get; set; }
        public bool IsFinal { get; set; }
        public bool IsActive { get; set; }
    }
}