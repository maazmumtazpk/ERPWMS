using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services.Workflow
{
    public interface IWorkflowStageService
    {
        // =========================================================
        // GET METHODS
        // =========================================================

        Task<IEnumerable<WorkflowStage>> GetStagesByDefinitionIdAsync(int definitionId, bool includeInactive = false);
        Task<IEnumerable<WorkflowStage>> GetByDefinitionIdAsync(int definitionId, bool includeInactive = false);  // ✅ Add this - for consistency
        Task<WorkflowStage?> GetStageByIdAsync(int id);
        Task<WorkflowStage?> GetByIdAsync(int id);  // ✅ Add this - for consistency
        Task<WorkflowStage?> GetByCodeAsync(int definitionId, string code);
        Task<IEnumerable<WorkflowStage>> GetActiveStagesByDefinitionIdAsync(int definitionId);
        Task<IEnumerable<WorkflowStage>> GetStagesWithTransitionsAsync(int definitionId);
        Task<WorkflowStage?> GetFirstStageAsync(int definitionId);
        Task<WorkflowStage?> GetFinalStageAsync(int definitionId);

        // =========================================================
        // CREATE METHODS
        // =========================================================

        Task<WorkflowStage> CreateStageAsync(WorkflowStage stage);
        Task AddAsync(WorkflowStage stage);  // ✅ Add this - for consistency

        // =========================================================
        // UPDATE METHODS
        // =========================================================

        Task UpdateStageAsync(WorkflowStage stage);
        Task UpdateAsync(WorkflowStage stage);  // ✅ Add this - for consistency
        Task<bool> ReorderStagesAsync(int definitionId, Dictionary<int, int> stageOrders);

        // =========================================================
        // DELETE METHODS
        // =========================================================

        Task<bool> DeleteStageAsync(int id);
        Task<bool> DeleteAsync(int id);  // ✅ Add this - for consistency
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> CanBeDeletedAsync(int id); // if any transitions or instances reference it

        // =========================================================
        // VALIDATION METHODS
        // =========================================================

        Task<bool> IsCodeUniqueAsync(int definitionId, string code, int? excludeId = null);
        Task<bool> IsStageOrderUniqueAsync(int definitionId, int stageOrder, int? excludeId = null);
        Task<bool> ValidateStageAsync(int id);
        Task<bool> HasTransitionsAsync(int stageId);
        Task<bool> HasInstancesAsync(int stageId);
        Task<bool> HasActiveAssignmentsAsync(int stageId);

        // =========================================================
        // BUSINESS METHODS
        // =========================================================

        Task<bool> CanMoveToNextStageAsync(int currentStageId, int targetStageId);
        Task<IEnumerable<WorkflowStage>> GetNextStagesAsync(int currentStageId);
        Task<bool> IsStageReachableAsync(int definitionId, int stageId);
        Task<int> GetStageOrderAsync(int stageId);
    }
}