using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IWorkflowEngineService
    {
        // Start a new workflow instance for a business entity
        Task<WorkflowInstance> StartWorkflowAsync(int definitionId, int entityId, string entityType, int initiatedBy);

        // Get the current stage of an instance
        Task<WorkflowStage?> GetCurrentStageAsync(int instanceId);

        // Get available actions for a user on a specific instance
        Task<IEnumerable<WorkflowTransition>> GetAvailableTransitionsAsync(int instanceId, int userId);

        // Execute a transition (action) on a workflow instance
        Task<WorkflowInstance> ExecuteTransitionAsync(int instanceId, int transitionId, int userId, string? comment = null);

        // Complete a workflow (if final stage reached or forced)
        Task<WorkflowInstance> CompleteWorkflowAsync(int instanceId, int userId, string? comment = null);

        // Reject/Cancel a workflow
        Task<WorkflowInstance> RejectWorkflowAsync(int instanceId, int userId, string? comment = null);

        // Cancel a workflow
        Task<WorkflowInstance> CancelWorkflowAsync(int instanceId, int userId, string? comment = null);

        // Get workflow history logs
        Task<IEnumerable<WorkflowInstanceLog>> GetWorkflowHistoryAsync(int instanceId);

        // Get assignments for an instance
        Task<IEnumerable<WorkflowAssignment>> GetAssignmentsAsync(int instanceId);
    }
}