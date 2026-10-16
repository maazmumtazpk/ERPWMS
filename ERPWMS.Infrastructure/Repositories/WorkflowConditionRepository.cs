using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class WorkflowConditionRepository : IWorkflowConditionRepository
    {
        private readonly AppDbContext _context;

        public WorkflowConditionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowCondition>> GetByDefinitionIdAsync(
            int definitionId,
            bool includeInactive = false)
        {
            return await _context.WorkflowConditions
                .Include(c => c.WorkflowDef)
                .Include(c => c.StageFrom)
                .Include(c => c.StageTo)
                .Where(c => c.WorkflowDefId == definitionId)
                .OrderBy(c => c.StageFromId)
                .ThenBy(c => c.StageToId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkflowCondition>> GetByTransitionIdAsync(
            int transitionId)
        {
            var transition = await _context.WorkflowTransitions
                .FirstOrDefaultAsync(t => t.TransitionId == transitionId);

            if (transition == null)
            {
                return Enumerable.Empty<WorkflowCondition>();
            }

            return await _context.WorkflowConditions
                .Include(c => c.WorkflowDef)
                .Include(c => c.StageFrom)
                .Include(c => c.StageTo)
                .Where(c =>
                    c.WorkflowDefId == transition.WorkflowDefId &&
                    c.StageFromId == transition.StageFromId &&
                    c.StageToId == transition.StageToId)
                .ToListAsync();
        }

        public async Task<WorkflowCondition?> GetByIdAsync(int id)
        {
            return await _context.WorkflowConditions
                .Include(c => c.WorkflowDef)
                .Include(c => c.StageFrom)
                .Include(c => c.StageTo)
                .FirstOrDefaultAsync(c => c.ConditionId == id);
        }

        public async Task AddAsync(WorkflowCondition entity)
        {
            entity.CreatedDate = DateTime.UtcNow;

            await _context.WorkflowConditions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowCondition entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _context.WorkflowConditions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
                return false;

            _context.WorkflowConditions.Remove(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

