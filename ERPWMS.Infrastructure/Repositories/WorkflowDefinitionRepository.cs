
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
    public class WorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private readonly AppDbContext _context;

        public WorkflowDefinitionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowDefinition>> GetAllAsync(
            bool includeInactive = false)
        {
            var query = _context.WorkflowDefinitions
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(w => w.IsActive == true);
            }

            return await query
                .OrderBy(w => w.WorkflowName)
                .ToListAsync();
        }

        public async Task<WorkflowDefinition?> GetByIdAsync(int id)
        {
            return await _context.WorkflowDefinitions
                .Include(w => w.WorkflowStages)
                .FirstOrDefaultAsync(w => w.WorkflowDefId == id);
        }

        public async Task<WorkflowDefinition?> GetByCodeAsync(string code)
        {
            // WorkflowDefinition model mein Code property nahi hai.
            // Isliye ModuleType ko code ke taur par use kiya gaya hai.
            return await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(w => w.ModuleType == code);
        }

        public async Task<bool> IsCodeUniqueAsync(
            string code,
            int? excludeId = null)
        {
            var query = _context.WorkflowDefinitions
                .Where(w => w.ModuleType == code);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    w => w.WorkflowDefId != excludeId);
            }

            return !await query.AnyAsync();
        }

        public async Task AddAsync(WorkflowDefinition entity)
        {
            entity.CreatedDate = DateTime.UtcNow;

            await _context.WorkflowDefinitions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowDefinition entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _context.WorkflowDefinitions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            // Aapke model mein IsDeleted available hai,
            // isliye soft delete use kar sakte hain.
            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.DeletedDate = DateTime.UtcNow;

            _context.WorkflowDefinitions.Update(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}


