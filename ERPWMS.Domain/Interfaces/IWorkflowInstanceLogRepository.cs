using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWorkflowInstanceLogRepository
    {
        Task<IEnumerable<WorkflowInstanceLog>> GetByInstanceIdAsync(int instanceId);
        Task AddAsync(WorkflowInstanceLog log);
    }
}
