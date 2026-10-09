using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class ChartOfAccountRepository : IChartOfAccountRepository
    {
        private readonly AppDbContext _context;

        public ChartOfAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChartOfAccount>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.ChartOfAccounts
                .Where(c => c.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(c => c.IsDeleted != true);

            return await query.OrderBy(c => c.AccountCode).ToListAsync();
        }

        public async Task<ChartOfAccount?> GetByIdAsync(int id, int companyId)
        {
            return await _context.ChartOfAccounts
                .FirstOrDefaultAsync(c =>
                    c.AccountId == id &&
                    c.CompanyId == companyId &&
                    c.IsDeleted != true);
        }

        public async Task<ChartOfAccount?> GetByCodeAsync(int companyId, string accountCode)
        {
            return await _context.ChartOfAccounts
                .FirstOrDefaultAsync(c =>
                    c.CompanyId == companyId &&
                    c.AccountCode == accountCode &&
                    c.IsDeleted != true);
        }

        public async Task<bool> IsCodeExistsAsync(int companyId, string accountCode, int? excludeId = null)
        {
            var query = _context.ChartOfAccounts
                .Where(c => c.CompanyId == companyId &&
                            c.AccountCode == accountCode &&
                            c.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(c => c.AccountId != excludeId);

            return await query.AnyAsync();
        }

        // ✅ FIXED: ParentId property remove kar di
        public Task<bool> HasChildrenAsync(int accountId, int companyId)
        {
            // TODO: Implement once ParentAccountId column is available
            return Task.FromResult(false);
        }

        // ✅ FIXED: ParentId property remove kar di
        public Task<bool> IsParentValidAsync(int companyId, int? parentId)
        {
            if (!parentId.HasValue) return Task.FromResult(true);

            // TODO: Validate parent once ParentAccountId column is available
            return Task.FromResult(true);
        }

        public async Task AddAsync(ChartOfAccount entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.ChartOfAccounts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChartOfAccount entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.ChartOfAccounts.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(id, companyId);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        public Task<bool> CanDeleteAsync(int accountId, int companyId)
        {
            // TODO: Implement child check once ParentAccountId is available
            return Task.FromResult(true);
        }
    }
}