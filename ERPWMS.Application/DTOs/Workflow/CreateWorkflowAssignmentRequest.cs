using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class CreateWorkflowAssignmentRequest
    {
        [Required]
        public int WorkflowInstanceId { get; set; }

        [Required]
        public int StageId { get; set; }

        public int? UserId { get; set; }
        public int? RoleId { get; set; }

        public bool CanApprove { get; set; }
        public bool CanReject { get; set; }
        public bool CanHold { get; set; }
        public bool CanSendBack { get; set; }
        public bool IsActive { get; set; } = true;
    }
}