using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowDefinitionRepository
    {
        Task<IEnumerable<WorkflowDefinition>> GetAllAsync(bool includeInactive = false);
        Task<WorkflowDefinition?> GetByIdAsync(int id);
        Task<WorkflowDefinition?> GetByCodeAsync(string code);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
        Task AddAsync(WorkflowDefinition entity);
        Task UpdateAsync(WorkflowDefinition entity);
        Task<bool> DeleteAsync(int id); // soft delete may exist, we'll implement accordingly
    }
}
