namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowDashboardDto
    {
        public int ActiveWorkflows { get; set; }
        public int PendingApprovals { get; set; }
        public int CompletedWorkflows { get; set; }
        public int RejectedWorkflows { get; set; }
        public int OverdueWorkflows { get; set; }
        public int EscalatedWorkflows { get; set; }
        public int TotalDefinitions { get; set; }
        public int TotalInstances { get; set; }
    }
}