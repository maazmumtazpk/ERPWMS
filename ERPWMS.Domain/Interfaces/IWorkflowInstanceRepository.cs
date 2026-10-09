using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowInstanceRepository
    {
        Task<IEnumerable<WorkflowInstance>> GetByDefinitionIdAsync(int definitionId, bool includeCompleted = false);
        Task<IEnumerable<WorkflowInstance>> GetByEntityAsync(string entityType, int entityId);
        Task<WorkflowInstance?> GetByIdAsync(int id);
        Task<WorkflowInstance?> GetActiveByEntityAsync(string entityType, int entityId, int definitionId);
        Task AddAsync(WorkflowInstance instance);
        Task UpdateAsync(WorkflowInstance instance);
        Task<bool> DeleteAsync(int id);
    }
}
