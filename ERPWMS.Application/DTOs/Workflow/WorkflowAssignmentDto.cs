namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowAssignmentDto
    {
        public int Id { get; set; }
        public int WorkflowInstanceId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public int StageId { get; set; }
        public string? StageName { get; set; }
        public bool CanApprove { get; set; }
        public bool CanReject { get; set; }
        public bool CanHold { get; set; }
        public bool CanSendBack { get; set; }
        public bool IsActive { get; set; }
    }
}