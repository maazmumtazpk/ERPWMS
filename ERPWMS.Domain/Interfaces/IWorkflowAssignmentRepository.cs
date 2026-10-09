using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowAssignmentRepository
    {
        Task<IEnumerable<WorkflowAssignment>> GetByInstanceIdAsync(int instanceId);
        Task<IEnumerable<WorkflowAssignment>> GetByStageIdAsync(int stageId);
        Task<IEnumerable<WorkflowAssignment>> GetByUserAsync(int userId);
        Task<WorkflowAssignment?> GetAssignmentAsync(int instanceId, int stageId, int? userId, int? roleId);
        Task AddAsync(WorkflowAssignment assignment);
        Task UpdateAsync(WorkflowAssignment assignment);
        Task<bool> DeleteAsync(int id);
    }
}
