using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Services;
using ERPWMS.Domain.Services.DTOs;   // ✅ Domain wala DTO (sirf 4 properties)
using ERPWMS.Domain.Services.Workflow;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services   // ✅ Folder ke saath match
{
    public class WorkflowDashboardService(AppDbContext context) : IWorkflowDashboardService
    {
        public async Task<WorkflowDashboardDto> GetDashboardDataAsync()
        {
            // Total workflows
            var totalWorkflows = await context.WorkflowInstances
                .CountAsync();

            // Pending workflows (Active)
            var pendingWorkflows = await context.WorkflowInstances
                .CountAsync(i => i.Status == "Active");

            // Completed workflows
            var completedWorkflows = await context.WorkflowInstances
                .CountAsync(i => i.Status == "Completed");

            // Rejected workflows
            var rejectedWorkflows = await context.WorkflowInstances
                .CountAsync(i => i.Status == "Rejected");

            // ✅ Domain DTO ke hisaab se return
            return new WorkflowDashboardDto
            {
                TotalWorkflows = totalWorkflows,
                PendingWorkflows = pendingWorkflows,
                CompletedWorkflows = completedWorkflows,
                RejectedWorkflows = rejectedWorkflows
            };
        }
    }
}