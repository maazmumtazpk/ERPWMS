using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowStageRepository
    {
        Task<IEnumerable<WorkflowStage>> GetByDefinitionIdAsync(int definitionId, bool includeInactive = false);
        Task<WorkflowStage?> GetByIdAsync(int id);
        Task<WorkflowStage?> GetByCodeAsync(int definitionId, string code);
        Task<bool> IsCodeUniqueAsync(int definitionId, string code, int? excludeId = null);
        Task AddAsync(WorkflowStage entity);
        Task UpdateAsync(WorkflowStage entity);
        Task<bool> DeleteAsync(int id);
    }
}
