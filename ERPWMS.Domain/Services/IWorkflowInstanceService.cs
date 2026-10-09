using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IWorkflowInstanceService
    {
        Task<IEnumerable<WorkflowInstance>> GetInstancesAsync(
            int? definitionId = null,
            string? status = null,
            bool includeCompleted = false);

        Task<WorkflowInstance?> GetInstanceByIdAsync(int id);

        Task<IEnumerable<WorkflowInstance>> GetInstancesByEntityAsync(
            string entityType,
            int entityId);

        Task<WorkflowInstance> CreateInstanceAsync(
            WorkflowInstance instance);

        Task UpdateInstanceAsync(
            WorkflowInstance instance);

        Task<bool> DeleteInstanceAsync(int id);
    }
}