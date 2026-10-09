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
    public class WorkflowStageRepository : IWorkflowStageRepository
    {
        private readonly AppDbContext _context;

        public WorkflowStageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowStage>> GetByDefinitionIdAsync(
            int definitionId,
            bool includeInactive = false)
        {
            var query = _context.WorkflowStages
                .Where(s => s.WorkflowDefId == definitionId);

            if (!includeInactive)
            {
                query = query.Where(s => s.IsActive == true);
            }

            return await query
                .OrderBy(s => s.StageOrder)
                .ToListAsync();
        }

        public async Task<WorkflowStage?> GetByIdAsync(int id)
        {
            return await _context.WorkflowStages
                .Include(s => s.WorkflowDef)
                .FirstOrDefaultAsync(s => s.StageId == id);
        }

        public async Task<WorkflowStage?> GetByCodeAsync(
            int definitionId,
            string code)
        {
            return await _context.WorkflowStages
                .FirstOrDefaultAsync(s =>
                    s.WorkflowDefId == definitionId &&
                    s.StageCode == code);
        }

        public async Task<bool> IsCodeUniqueAsync(
            int definitionId,
            string code,
            int? excludeId = null)
        {
            var query = _context.WorkflowStages
                .Where(s =>
                    s.WorkflowDefId == definitionId &&
                    s.StageCode == code);

            if (excludeId.HasValue)
            {
                query = query.Where(s =>
                    s.StageId != excludeId);
            }

            return !await query.AnyAsync();
        }

        public async Task AddAsync(WorkflowStage entity)
        {
            entity.CreatedDate = DateTime.UtcNow;

            await _context.WorkflowStages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowStage entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _context.WorkflowStages.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.DeletedDate = DateTime.UtcNow;

            _context.WorkflowStages.Update(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
