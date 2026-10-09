using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowDeadlineService(
        IWorkflowDeadlineRepository repository,
        IWorkflowInstanceRepository instanceRepo)
        : IWorkflowDeadlineService
    {
        private readonly IWorkflowDeadlineRepository _repository = repository;
        private readonly IWorkflowInstanceRepository _instanceRepo = instanceRepo;

        public async Task<IEnumerable<WorkflowDeadline>> GetDeadlinesByDefinitionAsync(
            int definitionId,
            bool includeInactive = false)
        {
            return await _repository.GetByDefinitionIdAsync(
                definitionId,
                includeInactive);
        }

        public async Task<WorkflowDeadline?> GetDeadlineByStageAsync(
            int stageId)
        {
            return await _repository.GetByStageIdAsync(stageId);
        }

        public async Task<WorkflowDeadline?> GetDeadlineByIdAsync(
            int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<WorkflowDeadline> CreateDeadlineAsync(
            WorkflowDeadline deadline)
        {
            var existing = await _repository.GetByStageIdAsync(
                deadline.StageId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    "A deadline already exists for this stage.");
            }

            await _repository.AddAsync(deadline);

            return deadline;
        }

        public async Task UpdateDeadlineAsync(
            WorkflowDeadline deadline)
        {
            await _repository.UpdateAsync(deadline);
        }

        public async Task<bool> DeleteDeadlineAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> IsDeadlineExceededAsync(
            int instanceId)
        {
            var instance = await _instanceRepo.GetByIdAsync(instanceId);

            if (instance == null ||
                instance.Status != "Active")
            {
                return false;
            }

            var deadline = await _repository.GetByStageIdAsync(
                instance.CurrentStageId);

            if (deadline == null)
                return false;

            if (deadline.IsMet == true)
                return false;

            return DateTime.UtcNow > deadline.DueDate;
        }

        public async Task HandleEscalationAsync(int instanceId)
        {
            var instance = await _instanceRepo.GetByIdAsync(instanceId);

            if (instance == null ||
                instance.Status != "Active")
            {
                return;
            }

            var deadline = await _repository.GetByStageIdAsync(
                instance.CurrentStageId);

            if (deadline == null)
                return;

            if (deadline.IsMet == true)
                return;

            if (DateTime.UtcNow <= deadline.DueDate)
                return;

            if (deadline.EscalatedAt != null)
                return;

            deadline.EscalatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(deadline);
        }
    }
}
