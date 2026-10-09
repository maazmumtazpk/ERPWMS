using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IWorkflowConditionService
    {
        Task<IEnumerable<WorkflowCondition>> GetConditionsByDefinitionIdAsync(int definitionId, bool includeInactive = false);
        Task<IEnumerable<WorkflowCondition>> GetConditionsByTransitionIdAsync(int transitionId);
        Task<WorkflowCondition?> GetConditionByIdAsync(int id);
        Task<WorkflowCondition> CreateConditionAsync(WorkflowCondition condition);
        Task UpdateConditionAsync(WorkflowCondition condition);
        Task<bool> DeleteConditionAsync(int id);
        Task<bool> EvaluateConditionAsync(WorkflowCondition condition, object data);
    }
}