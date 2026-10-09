
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
    public class WorkflowTransitionRepository : IWorkflowTransitionRepository
    {
        private readonly AppDbContext _context;

        public WorkflowTransitionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowTransition>> GetByDefinitionIdAsync(
            int definitionId,
            bool includeInactive = false)
        {
            var query = _context.WorkflowTransitions
                .Include(t => t.StageFrom)
                .Include(t => t.StageTo)
                .Include(t => t.Action)
                .Where(t => t.WorkflowDefId == definitionId);

            if (!includeInactive)
            {
                query = query.Where(t => t.IsAllowed == true);
            }

            return await query
                .OrderBy(t => t.StageFromId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkflowTransition>> GetBySourceStageIdAsync(
            int stageId)
        {
            return await _context.WorkflowTransitions
                .Include(t => t.StageFrom)
                .Include(t => t.StageTo)
                .Include(t => t.Action)
                .Where(t =>
                    t.StageFromId == stageId &&
                    t.IsAllowed == true)
                .ToListAsync();
        }

        public async Task<WorkflowTransition?> GetByIdAsync(int id)
        {
            return await _context.WorkflowTransitions
                .Include(t => t.StageFrom)
                .Include(t => t.StageTo)
                .Include(t => t.Action)
                .Include(t => t.WorkflowDef)
                .FirstOrDefaultAsync(t => t.TransitionId == id);
        }

        public async Task<bool> IsTransitionUniqueAsync(
            int definitionId,
            int sourceStageId,
            int actionId,
            int? excludeId = null)
        {
            var query = _context.WorkflowTransitions
                .Where(t =>
                    t.WorkflowDefId == definitionId &&
                    t.StageFromId == sourceStageId &&
                    t.ActionId == actionId);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    t => t.TransitionId != excludeId);
            }

            return !await query.AnyAsync();
        }

        public async Task AddAsync(WorkflowTransition entity)
        {
            entity.CreatedDate = DateTime.UtcNow;

            await _context.WorkflowTransitions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowTransition entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _context.WorkflowTransitions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            // Soft disable transition
            entity.IsAllowed = false;
            entity.UpdatedDate = DateTime.UtcNow;

            _context.WorkflowTransitions.Update(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}


