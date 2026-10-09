using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowTransitionRepository
    {
        Task<IEnumerable<WorkflowTransition>> GetByDefinitionIdAsync(int definitionId, bool includeInactive = false);
        Task<IEnumerable<WorkflowTransition>> GetBySourceStageIdAsync(int stageId);
        Task<WorkflowTransition?> GetByIdAsync(int id);
        Task<bool> IsTransitionUniqueAsync(int definitionId, int sourceStageId, int actionId, int? excludeId = null);
        Task AddAsync(WorkflowTransition entity);
        Task UpdateAsync(WorkflowTransition entity);
        Task<bool> DeleteAsync(int id);
    }
}
