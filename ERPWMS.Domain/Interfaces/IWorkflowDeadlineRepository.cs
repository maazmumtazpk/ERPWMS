using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowDeadlineRepository
    {
        Task<IEnumerable<WorkflowDeadline>> GetByDefinitionIdAsync(int definitionId, bool includeInactive = false);
        Task<WorkflowDeadline?> GetByStageIdAsync(int stageId);
        Task<WorkflowDeadline?> GetByIdAsync(int id);
        Task AddAsync(WorkflowDeadline deadline);
        Task UpdateAsync(WorkflowDeadline deadline);
        Task<bool> DeleteAsync(int id);
    }
}
