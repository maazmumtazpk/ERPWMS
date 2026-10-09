using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class UpdateWorkflowDefinitionRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is required.")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Code can only contain letters, numbers, and underscores.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? EntityType { get; set; }

        public bool IsActive { get; set; }
        public int? StartStageId { get; set; }
        public int? FinalStageId { get; set; }
    }
}