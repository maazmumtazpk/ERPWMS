
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services.Workflow
{
    public class WorkflowInstanceLogService : IWorkflowInstanceLogService
    {
        private readonly IWorkflowInstanceLogRepository _repository;

        public WorkflowInstanceLogService(
            IWorkflowInstanceLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<WorkflowInstanceLog>> GetByInstanceIdAsync(
            int instanceId)
        {
            return await _repository.GetByInstanceIdAsync(instanceId);
        }

        public async Task AddAsync(
            WorkflowInstanceLog log)
        {
            await _repository.AddAsync(log);
        }
    }
}

