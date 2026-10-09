using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services.Workflow;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowDefinitionService : IWorkflowDefinitionService
    {
        private readonly IWorkflowDefinitionRepository _repository;
        private readonly IWorkflowStageRepository _stageRepository;
        private readonly IWorkflowInstanceRepository _instanceRepository;

        public WorkflowDefinitionService(
            IWorkflowDefinitionRepository repository,
            IWorkflowStageRepository stageRepository,
            IWorkflowInstanceRepository instanceRepository)
        {
            _repository = repository;
            _stageRepository = stageRepository;
            _instanceRepository = instanceRepository;
        }

        // =========================================================
        // GET
        // =========================================================

        public async Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync(
            bool includeInactive = false)
        {
            return await _repository.GetAllAsync(includeInactive);
        }

        public async Task<WorkflowDefinition?> GetDefinitionByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<WorkflowDefinition?> GetByIdAsync(int id)
        {
            return await GetDefinitionByIdAsync(id);
        }

        public async Task<WorkflowDefinition?> GetByCodeAsync(string code)
        {
            return await _repository.GetByCodeAsync(code);
        }

        public async Task<IEnumerable<WorkflowDefinition>> GetActiveDefinitionsAsync()
        {
            return await _repository.GetAllAsync(false);
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task<WorkflowDefinition> CreateDefinitionAsync(
            WorkflowDefinition definition)
        {
            if (!await _repository.IsCodeUniqueAsync(definition.ModuleType))
            {
                throw new InvalidOperationException(
                    $"A workflow with code '{definition.ModuleType}' already exists.");
            }

            definition.CreatedDate = DateTime.UtcNow;

            await _repository.AddAsync(definition);

            return definition;
        }

        public async Task AddAsync(WorkflowDefinition definition)
        {
            await CreateDefinitionAsync(definition);
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateDefinitionAsync(
            WorkflowDefinition definition)
        {
            if (!await _repository.IsCodeUniqueAsync(
                    definition.ModuleType,
                    definition.WorkflowDefId))
            {
                throw new InvalidOperationException(
                    $"A workflow with code '{definition.ModuleType}' already exists.");
            }

            definition.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(definition);
        }

        public async Task UpdateAsync(WorkflowDefinition definition)
        {
            await UpdateDefinitionAsync(definition);
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteDefinitionAsync(int id)
        {
            if (!await CanBeDeletedAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot delete this definition because it has workflow instances.");
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await DeleteDefinitionAsync(id);
        }

        public async Task<bool> CanBeDeletedAsync(int id)
        {
            var instances =
                await _instanceRepository.GetByDefinitionIdAsync(id);

            return !instances.Any();
        }

        // =========================================================
        // VALIDATION
        // =========================================================

        public async Task<bool> IsCodeUniqueAsync(
            string code,
            int? excludeId = null)
        {
            return await _repository.IsCodeUniqueAsync(
                code,
                excludeId);
        }

        public async Task<bool> ValidateDefinitionAsync(int id)
        {
            var definition = await _repository.GetByIdAsync(id);

            if (definition == null)
                return false;

            var stages =
                await _stageRepository.GetByDefinitionIdAsync(id);

            return stages != null && stages.Any();
        }

        public async Task<bool> HasInstancesAsync(int id)
        {
            var instances =
                await _instanceRepository.GetByDefinitionIdAsync(id);

            return instances.Any();
        }

        // =========================================================
        // COUNTS
        // =========================================================

        public async Task<int> GetInstanceCountAsync(int definitionId)
        {
            var instances =
                await _instanceRepository.GetByDefinitionIdAsync(definitionId);

            return instances.Count();
        }

        public async Task<int> GetActiveInstanceCountAsync(int definitionId)
        {
            var instances =
                await _instanceRepository.GetByDefinitionIdAsync(definitionId);

            return instances.Count(i =>
                i.Status == "InProgress");
        }
    }
}
