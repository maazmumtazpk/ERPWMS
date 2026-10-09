using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services.Workflow
{
    public interface IWorkflowActionService
    {
        Task<IEnumerable<WorkflowAction>> GetAllActionsAsync(bool includeInactive = false);
        Task<WorkflowAction?> GetActionByIdAsync(int id);
    }
}

