namespace ERPWMS.Domain.Services.DTOs
{
    public class WorkflowDashboardDto
    {
        public int TotalWorkflows { get; set; }
        public int PendingWorkflows { get; set; }
        public int CompletedWorkflows { get; set; }
        public int RejectedWorkflows { get; set; }
    }
}