using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IWorkflowTransitionService
    {
        Task<IEnumerable<WorkflowTransition>> GetTransitionsByDefinitionIdAsync(
            int definitionId,
            bool includeInactive = false);

        Task<IEnumerable<WorkflowTransition>> GetTransitionsBySourceStageIdAsync(
            int stageId);

        Task<WorkflowTransition?> GetTransitionByIdAsync(int id);

        Task<WorkflowTransition> CreateTransitionAsync(
            WorkflowTransition transition);

        Task UpdateTransitionAsync(
            WorkflowTransition transition);

        Task<bool> DeleteTransitionAsync(int id);

        Task<bool> IsTransitionUniqueAsync(
            int definitionId,
            int sourceStageId,
            int actionId,
            int? excludeId = null);
    }
}