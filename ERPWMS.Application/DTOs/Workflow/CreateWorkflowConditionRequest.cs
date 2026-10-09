
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class CreateWorkflowConditionRequest
    {
        [Required(ErrorMessage = "Workflow Definition is required.")]
        public int WorkflowDefinitionId { get; set; }

        [Required(ErrorMessage = "From Stage is required.")]
        public int StageFromId { get; set; }

        [Required(ErrorMessage = "To Stage is required.")]
        public int StageToId { get; set; }

        [Required(ErrorMessage = "Condition type is required.")]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Operator is required.")]
        [StringLength(20)]
        public string Operator { get; set; } = string.Empty;

        [Required(ErrorMessage = "Value is required.")]
        [StringLength(255)]
        public string Value { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ActionOnFail { get; set; }
    }
}

