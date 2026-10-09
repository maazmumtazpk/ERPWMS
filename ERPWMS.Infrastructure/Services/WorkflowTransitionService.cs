using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowTransitionService(
        IWorkflowTransitionRepository repository,
        IWorkflowStageRepository stageRepository) : IWorkflowTransitionService
    {
        private readonly IWorkflowTransitionRepository _repository = repository;
        private readonly IWorkflowStageRepository _stageRepository = stageRepository;

        public async Task<IEnumerable<WorkflowTransition>> GetTransitionsByDefinitionIdAsync(
            int definitionId,
            bool includeInactive = false)
        {
            return await _repository.GetByDefinitionIdAsync(
                definitionId,
                includeInactive);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetTransitionsBySourceStageIdAsync(
            int stageId)
        {
            return await _repository.GetBySourceStageIdAsync(stageId);
        }

        public async Task<WorkflowTransition?> GetTransitionByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<WorkflowTransition> CreateTransitionAsync(
            WorkflowTransition transition)
        {
            var sourceStage = await _stageRepository.GetByIdAsync(
                transition.StageFromId);

            var destinationStage = await _stageRepository.GetByIdAsync(
                transition.StageToId);

            if (sourceStage == null || destinationStage == null)
            {
                throw new Exception(
                    "Invalid source or destination stage.");
            }

            if (sourceStage.WorkflowDefId != transition.WorkflowDefId ||
                destinationStage.WorkflowDefId != transition.WorkflowDefId)
            {
                throw new Exception(
                    "Source and destination stages must belong to the same workflow definition.");
            }

            var isUnique = await _repository.IsTransitionUniqueAsync(
                transition.WorkflowDefId,
                transition.StageFromId,
                transition.ActionId);

            if (!isUnique)
            {
                throw new Exception(
                    "A transition with the same source stage and action already exists.");
            }

            await _repository.AddAsync(transition);

            return transition;
        }

        public async Task UpdateTransitionAsync(
            WorkflowTransition transition)
        {
            var isUnique = await _repository.IsTransitionUniqueAsync(
                transition.WorkflowDefId,
                transition.StageFromId,
                transition.ActionId,
                transition.TransitionId);

            if (!isUnique)
            {
                throw new Exception(
                    "A transition with the same source stage and action already exists.");
            }

            await _repository.UpdateAsync(transition);
        }

        public async Task<bool> DeleteTransitionAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> IsTransitionUniqueAsync(
            int definitionId,
            int sourceStageId,
            int actionId,
            int? excludeId = null)
        {
            return await _repository.IsTransitionUniqueAsync(
                definitionId,
                sourceStageId,
                actionId,
                excludeId);
        }
    }
}
