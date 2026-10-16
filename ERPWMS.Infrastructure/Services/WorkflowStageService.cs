using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services.Workflow;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowStageService(
        IWorkflowStageRepository repository,
        IWorkflowTransitionRepository transitionRepository,
        IWorkflowInstanceRepository instanceRepository,
        IWorkflowAssignmentRepository assignmentRepository) : IWorkflowStageService
    {
        private readonly IWorkflowStageRepository _repository = repository;
        private readonly IWorkflowTransitionRepository _transitionRepository = transitionRepository;
        private readonly IWorkflowInstanceRepository _instanceRepository = instanceRepository;
        private readonly IWorkflowAssignmentRepository _assignmentRepository = assignmentRepository;

        // =========================================================
        // GET
        // =========================================================

        public async Task<IEnumerable<WorkflowStage>> GetStagesByDefinitionIdAsync(int definitionId, bool includeInactive = false)
            => await _repository.GetByDefinitionIdAsync(definitionId, includeInactive);

        public async Task<IEnumerable<WorkflowStage>> GetByDefinitionIdAsync(int definitionId, bool includeInactive = false)
            => await GetStagesByDefinitionIdAsync(definitionId, includeInactive);

        public async Task<WorkflowStage?> GetStageByIdAsync(int id)
            => await _repository.GetByIdAsync(id);

        public async Task<WorkflowStage?> GetByIdAsync(int id)
            => await GetStageByIdAsync(id);

        public async Task<WorkflowStage?> GetByCodeAsync(int definitionId, string code)
        {
            var stages = await _repository.GetByDefinitionIdAsync(definitionId, true);
            return stages.FirstOrDefault(s => s.StageCode == code);
        }

        public async Task<IEnumerable<WorkflowStage>> GetActiveStagesByDefinitionIdAsync(int definitionId)
            => await _repository.GetByDefinitionIdAsync(definitionId, false);

        public async Task<IEnumerable<WorkflowStage>> GetStagesWithTransitionsAsync(int definitionId)
            => await _repository.GetByDefinitionIdAsync(definitionId, false);

        public async Task<WorkflowStage?> GetFirstStageAsync(int definitionId)
        {
            var stages = await _repository.GetByDefinitionIdAsync(definitionId, false);
            return stages.OrderBy(s => s.StageOrder).FirstOrDefault();
        }

        public async Task<WorkflowStage?> GetFinalStageAsync(int definitionId)
        {
            var stages = await _repository.GetByDefinitionIdAsync(definitionId, false);
            return stages.FirstOrDefault(s => s.IsEnd == true);
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task<WorkflowStage> CreateStageAsync(WorkflowStage stage)
        {
            if (!await _repository.IsCodeUniqueAsync(stage.WorkflowDefId, stage.StageCode))
                throw new InvalidOperationException($"A stage with code '{stage.StageCode}' already exists in this workflow.");

            await _repository.AddAsync(stage);
            return stage;
        }

        public async Task AddAsync(WorkflowStage stage)
            => await CreateStageAsync(stage);

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateStageAsync(WorkflowStage stage)
        {
            if (!await _repository.IsCodeUniqueAsync(stage.WorkflowDefId, stage.StageCode, stage.StageId))
                throw new InvalidOperationException($"A stage with code '{stage.StageCode}' already exists in this workflow.");

            await _repository.UpdateAsync(stage);
        }

        public async Task UpdateAsync(WorkflowStage stage)
            => await UpdateStageAsync(stage);

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteStageAsync(int id)
        {
            if (!await CanBeDeletedAsync(id))
                throw new InvalidOperationException("Cannot delete this stage because it is referenced by workflow transitions or has active instances.");

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
            => await DeleteStageAsync(id);

        public async Task<bool> SoftDeleteAsync(int id)
            => await _repository.DeleteAsync(id);

        // =========================================================
        // VALIDATION
        // =========================================================

        public async Task<bool> IsCodeUniqueAsync(int definitionId, string code, int? excludeId = null)
            => await _repository.IsCodeUniqueAsync(definitionId, code, excludeId);

        public async Task<bool> IsStageOrderUniqueAsync(int definitionId, int stageOrder, int? excludeId = null)
        {
            var stages = await _repository.GetByDefinitionIdAsync(definitionId, true);
            return !stages.Any(s =>
                s.StageOrder == stageOrder &&
                (!excludeId.HasValue || s.StageId != excludeId));
        }

        public async Task<bool> ValidateStageAsync(int id)
        {
            var stage = await _repository.GetByIdAsync(id);
            if (stage == null) return false;
            return !string.IsNullOrWhiteSpace(stage.StageCode);
        }

        // ✅ FIXED: Real implementation
        public async Task<bool> HasTransitionsAsync(int id)
        {
            var stage = await _repository.GetByIdAsync(id);
            if (stage == null) return false;

            var transitions = await _transitionRepository
                .GetByDefinitionIdAsync(stage.WorkflowDefId, true);

            return transitions.Any(t =>
                t.StageFromId == id || t.StageToId == id);
        }

        // ✅ FIXED: Real implementation
        public async Task<bool> HasInstancesAsync(int id)
        {
            var stage = await _repository.GetByIdAsync(id);
            if (stage == null) return false;

            var instances = await _instanceRepository
                .GetByDefinitionIdAsync(stage.WorkflowDefId, true);

            return instances.Any(i => i.CurrentStageId == id);
        }

        // ✅ FIXED: Real implementation
        public async Task<bool> HasActiveAssignmentsAsync(int id)
        {
            var assignments = await _assignmentRepository
                .GetByStageIdAsync(id);

            return assignments.Any(a => a.IsActive == true);
        }

        public async Task<bool> CanBeDeletedAsync(int id)
        {
            if (await HasTransitionsAsync(id)) return false;
            if (await HasInstancesAsync(id)) return false;
            if (await HasActiveAssignmentsAsync(id)) return false;
            return true;
        }

        // =========================================================
        // STAGE ORDER
        // =========================================================

        public async Task<bool> ReorderStagesAsync(int definitionId, Dictionary<int, int> stageOrders)
        {
            var stages = await _repository.GetByDefinitionIdAsync(definitionId, true);

            foreach (var stage in stages)
            {
                if (stageOrders.TryGetValue(stage.StageId, out var newOrder))
                {
                    stage.StageOrder = newOrder;
                    await _repository.UpdateAsync(stage);
                }
            }

            return true;
        }

        public async Task<int> GetStageOrderAsync(int stageId)
        {
            var stage = await _repository.GetByIdAsync(stageId);
            if (stage == null)
                throw new KeyNotFoundException($"Workflow stage with ID {stageId} was not found.");
            return stage.StageOrder;
        }

        // =========================================================
        // NAVIGATION
        // =========================================================

        public async Task<bool> CanMoveToNextStageAsync(int currentStageId, int targetStageId)
        {
            var transitions = await _transitionRepository
                .GetBySourceStageIdAsync(currentStageId);

            return transitions.Any(t =>
                t.StageToId == targetStageId &&
                t.IsAllowed != false);
        }

        public async Task<IEnumerable<WorkflowStage>> GetNextStagesAsync(int stageId)
        {
            var transitions = await _transitionRepository.GetBySourceStageIdAsync(stageId);
            var result = new List<WorkflowStage>();

            foreach (var transition in transitions)
            {
                var stage = await _repository.GetByIdAsync(transition.StageToId);
                if (stage != null) result.Add(stage);
            }

            return result;
        }

        public async Task<bool> IsStageReachableAsync(int fromStageId, int toStageId)
        {
            var visited = new HashSet<int>();
            var queue = new Queue<int>();

            queue.Enqueue(fromStageId);

            while (queue.Count > 0)
            {
                var currentStageId = queue.Dequeue();

                if (!visited.Add(currentStageId)) continue;
                if (currentStageId == toStageId) return true;

                var transitions = await _transitionRepository
                    .GetBySourceStageIdAsync(currentStageId);

                foreach (var transition in transitions)
                    queue.Enqueue(transition.StageToId);
            }

            return false;
        }
    }
}
