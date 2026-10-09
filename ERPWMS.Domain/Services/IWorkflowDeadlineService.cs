using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IWorkflowDeadlineService
    {
        Task<IEnumerable<WorkflowDeadline>> GetDeadlinesByDefinitionAsync(int definitionId, bool includeInactive = false);
        Task<WorkflowDeadline?> GetDeadlineByStageAsync(int stageId);
        Task<WorkflowDeadline?> GetDeadlineByIdAsync(int id);
        Task<WorkflowDeadline> CreateDeadlineAsync(WorkflowDeadline deadline);
        Task UpdateDeadlineAsync(WorkflowDeadline deadline);
        Task<bool> DeleteDeadlineAsync(int id);
        Task<bool> IsDeadlineExceededAsync(int instanceId);
        Task HandleEscalationAsync(int instanceId);
    }
}