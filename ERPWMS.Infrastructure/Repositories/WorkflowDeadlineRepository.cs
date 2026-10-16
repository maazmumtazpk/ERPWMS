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
    public class WorkflowDeadlineRepository : IWorkflowDeadlineRepository
    {
        private readonly AppDbContext _context;

        public WorkflowDeadlineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowDeadline>> GetByDefinitionIdAsync(
            int definitionId,
            bool includeInactive = false)
        {
            return await _context.WorkflowDeadlines
                .Include(d => d.Stage)
                .Include(d => d.WorkflowInst)
                .Where(d => d.WorkflowInst.WorkflowDefId == definitionId)
                .OrderByDescending(d => d.DueDate)
                .ToListAsync();
        }

        public async Task<WorkflowDeadline?> GetByStageIdAsync(int stageId)
        {
            return await _context.WorkflowDeadlines
                .Include(d => d.Stage)
                .Include(d => d.WorkflowInst)
                .Where(d => d.StageId == stageId)
                .OrderByDescending(d => d.DueDate)
                .FirstOrDefaultAsync();
        }

        public async Task<WorkflowDeadline?> GetByIdAsync(int id)
        {
            return await _context.WorkflowDeadlines
                .Include(d => d.Stage)
                .Include(d => d.WorkflowInst)
                .FirstOrDefaultAsync(d => d.DeadlineId == id);
        }

        public async Task AddAsync(WorkflowDeadline deadline)
        {
            deadline.CreatedDate = DateTime.UtcNow;

            await _context.WorkflowDeadlines.AddAsync(deadline);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowDeadline deadline)
        {
            deadline.UpdatedDate = DateTime.UtcNow;

            _context.WorkflowDeadlines.Update(deadline);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
                return false;

            _context.WorkflowDeadlines.Remove(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

