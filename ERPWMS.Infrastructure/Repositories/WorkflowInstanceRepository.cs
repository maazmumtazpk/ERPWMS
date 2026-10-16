using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories.Workflow
{
    public class WorkflowInstanceRepository : IWorkflowInstanceRepository
    {
        private readonly AppDbContext _context;

        public WorkflowInstanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByDefinitionIdAsync(
            int definitionId,
            bool includeCompleted = false)
        {
            var query = _context.WorkflowInstances
                .Include(i => i.WorkflowDef)
                .Include(i => i.CurrentStage)
                .Where(i => i.WorkflowDefId == definitionId);

            if (!includeCompleted)
            {
                query = query.Where(i => i.Status != "Completed");
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByEntityAsync(
            string entityType,
            int entityId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowDef)
                .Include(i => i.CurrentStage)
                .Where(i =>
                    i.RecordType == entityType &&
                    i.RecordId == entityId)
                .ToListAsync();
        }

        public async Task<WorkflowInstance?> GetByIdAsync(int id)
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowDef)
                .Include(i => i.CurrentStage)
                .FirstOrDefaultAsync(
                    i => i.WorkflowInstId == id);
        }

        public async Task<WorkflowInstance?> GetActiveByEntityAsync(
            string entityType,
            int entityId,
            int definitionId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowDef)
                .Include(i => i.CurrentStage)
                .FirstOrDefaultAsync(i =>
                    i.RecordType == entityType &&
                    i.RecordId == entityId &&
                    i.WorkflowDefId == definitionId &&
                    i.Status == "Active");
        }

        public async Task AddAsync(WorkflowInstance instance)
        {
            instance.CreatedDate = DateTime.UtcNow;

            await _context.WorkflowInstances.AddAsync(instance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowInstance instance)
        {
            instance.UpdatedDate = DateTime.UtcNow;

            _context.WorkflowInstances.Update(instance);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
                return false;

            _context.WorkflowInstances.Remove(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

