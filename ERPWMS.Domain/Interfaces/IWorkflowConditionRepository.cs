using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowConditionRepository
    {
        Task<IEnumerable<WorkflowCondition>> GetByDefinitionIdAsync(int definitionId, bool includeInactive = false);
        Task<IEnumerable<WorkflowCondition>> GetByTransitionIdAsync(int transitionId);
        Task<WorkflowCondition?> GetByIdAsync(int id);
        Task AddAsync(WorkflowCondition entity);
        Task UpdateAsync(WorkflowCondition entity);
        Task<bool> DeleteAsync(int id);
    }
}
