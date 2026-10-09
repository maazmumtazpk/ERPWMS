
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowInstanceService(
        IWorkflowInstanceRepository repository) : IWorkflowInstanceService
    {
        private readonly IWorkflowInstanceRepository _repository = repository;

        // =========================================================
        // GET INSTANCES
        // =========================================================

        public async Task<IEnumerable<WorkflowInstance>> GetInstancesAsync(
            int? definitionId = null,
            string? status = null,
            bool includeCompleted = false)
        {
            var instances = await _repository.GetByDefinitionIdAsync(
                definitionId ?? 0,
                includeCompleted);

            if (!string.IsNullOrWhiteSpace(status))
            {
                instances = instances.Where(i =>
                    i.Status == status);
            }

            return instances;
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<WorkflowInstance?> GetInstanceByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // =========================================================
        // GET BY ENTITY
        // =========================================================

        public async Task<IEnumerable<WorkflowInstance>>
            GetInstancesByEntityAsync(
                string entityType,
                int entityId)
        {
            return await _repository.GetByEntityAsync(
                entityType,
                entityId);
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task<WorkflowInstance>
            CreateInstanceAsync(
                WorkflowInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            instance.CreatedDate = DateTime.UtcNow;

            await _repository.AddAsync(instance);

            return instance;
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateInstanceAsync(
            WorkflowInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            instance.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(instance);
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteInstanceAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}


