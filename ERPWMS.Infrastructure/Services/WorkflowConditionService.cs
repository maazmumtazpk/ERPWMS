
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services.Workflow
{
    public class WorkflowConditionService : IWorkflowConditionService
    {
        private readonly IWorkflowConditionRepository _repository;
        private readonly IWorkflowDefinitionRepository _definitionRepository;

        public WorkflowConditionService(
            IWorkflowConditionRepository repository,
            IWorkflowDefinitionRepository definitionRepository)
        {
            _repository = repository;
            _definitionRepository = definitionRepository;
        }

        public async Task<IEnumerable<WorkflowCondition>> GetConditionsByDefinitionIdAsync(
            int definitionId,
            bool includeInactive = false)
        {
            return await _repository.GetByDefinitionIdAsync(
                definitionId,
                includeInactive);
        }

        public async Task<IEnumerable<WorkflowCondition>> GetConditionsByTransitionIdAsync(
            int transitionId)
        {
            return await _repository.GetByTransitionIdAsync(
                transitionId);
        }

        public async Task<WorkflowCondition?> GetConditionByIdAsync(
            int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<WorkflowCondition> CreateConditionAsync(
            WorkflowCondition condition)
        {
            // Basic validation:
            // WorkflowDefinition must exist before creating condition.
            var definition = await _definitionRepository.GetByIdAsync(
                condition.WorkflowDefId);

            if (definition == null)
            {
                throw new InvalidOperationException(
     "Workflow definition not found.");
            }

            await _repository.AddAsync(condition);

            return condition;
        }

        public async Task UpdateConditionAsync(
            WorkflowCondition condition)
        {
            await _repository.UpdateAsync(condition);
        }

        public async Task<bool> DeleteConditionAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> EvaluateConditionAsync(
            WorkflowCondition condition,
            object data)
        {
            // Temporary implementation.
            // Actual condition evaluation can be implemented later.
            return true;
        }
    }
}


