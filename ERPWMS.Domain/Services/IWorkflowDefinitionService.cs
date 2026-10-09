using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services.Workflow
{
    public interface IWorkflowDefinitionService
    {
        // GET Methods
        Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync(bool includeInactive = false);
        Task<WorkflowDefinition?> GetDefinitionByIdAsync(int id);
        Task<WorkflowDefinition?> GetByIdAsync(int id);  // ✅ Add this - for consistency
        Task<WorkflowDefinition?> GetByCodeAsync(string code);
        Task<IEnumerable<WorkflowDefinition>> GetActiveDefinitionsAsync();

        // CREATE Methods
        Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition);
        Task AddAsync(WorkflowDefinition definition);  // ✅ Add this - for consistency

        // UPDATE Methods
        Task UpdateDefinitionAsync(WorkflowDefinition definition);
        Task UpdateAsync(WorkflowDefinition definition);  // ✅ Add this - for consistency

        // DELETE Methods
        Task<bool> DeleteDefinitionAsync(int id);
        Task<bool> DeleteAsync(int id);  // ✅ Add this - for consistency
        Task<bool> CanBeDeletedAsync(int id); // check if any instances exist

        // VALIDATION Methods
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
        Task<bool> ValidateDefinitionAsync(int id);
        Task<bool> HasInstancesAsync(int id);  // ✅ Add this

        // COUNT Methods
        Task<int> GetInstanceCountAsync(int definitionId);
        Task<int> GetActiveInstanceCountAsync(int definitionId);
    }
}