using ERPWMS.Domain.Services.DTOs;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services.Workflow
{
    public interface IWorkflowDashboardService
    {
        Task<WorkflowDashboardDto> GetDashboardDataAsync();
    }
}