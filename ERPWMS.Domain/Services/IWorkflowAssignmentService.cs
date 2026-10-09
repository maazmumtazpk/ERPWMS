using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services.Workflow
{
    public interface IWorkflowAssignmentService
    {
        Task<IEnumerable<WorkflowAssignment>> GetAssignmentsByInstanceAsync(
            int instanceId);

        Task<IEnumerable<WorkflowAssignment>> GetAssignmentsByStageAsync(
            int stageId);

        Task<IEnumerable<WorkflowAssignment>> GetAssignmentsByInstanceAndStageAsync(
            int instanceId,
            int stageId);

        Task<IEnumerable<WorkflowAssignment>> GetAssignmentsByDefinitionAndStageAsync(
            int definitionId,
            int stageId);

        Task<WorkflowAssignment?> GetAssignmentAsync(
            int instanceId,
            int stageId,
            int? userId,
            int? roleId);

        Task<WorkflowAssignment> CreateAssignmentAsync(
            WorkflowAssignment assignment);

        Task UpdateAssignmentAsync(
            WorkflowAssignment assignment);

        Task<bool> DeleteAssignmentAsync(int id);

        Task<bool> HasAssignmentForActionAsync(
            int instanceId,
            int stageId,
            int userId,
            string actionType);
    }
}