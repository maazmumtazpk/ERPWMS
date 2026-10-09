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
    public class GrnRepository : IGrnRepository
    {
        private readonly AppDbContext _context;

        public GrnRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Grn>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.Grns
                .Include(g => g.Purchase)
                .Include(g => g.Warehouse)
                .Where(g => g.Warehouse.Branch.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(g => g.IsDeleted != true);

            return await query.OrderByDescending(g => g.GrnId).ToListAsync();
        }

        public async Task<Grn?> GetByIdAsync(int grnId, int companyId)
        {
            return await _context.Grns
                .Include(g => g.Purchase)
                .Include(g => g.Warehouse)
                .Include(g => g.Grndetails)
                .FirstOrDefaultAsync(g =>
                    g.GrnId == grnId &&
                    g.Warehouse.Branch.CompanyId == companyId);
        }

        public async Task<Grn?> GetByGrnNoAsync(string grnNo, int companyId)
        {
            return await _context.Grns
                .Include(g => g.Purchase)
                .FirstOrDefaultAsync(g =>
                    g.GrnNo == grnNo &&
                    g.Warehouse.Branch.CompanyId == companyId &&
                    g.IsDeleted != true);
        }

        public async Task<IEnumerable<Grn>> GetByPurchaseIdAsync(int purchaseId)
        {
            return await _context.Grns
                .Where(g => g.PurchaseId == purchaseId && g.IsDeleted != true)
                .OrderByDescending(g => g.GrnId)
                .ToListAsync();
        }

        // ✅ FIXED: GrnDate → CreatedDate use kiya
        public async Task<IEnumerable<Grn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            var query = _context.Grns
                .Where(g => g.Warehouse.Branch.CompanyId == companyId && g.IsDeleted != true);

            if (fromDate.HasValue) query = query.Where(g => g.CreatedDate >= fromDate);
            if (toDate.HasValue) query = query.Where(g => g.CreatedDate <= toDate);

            return await query.OrderByDescending(g => g.GrnId).ToListAsync();
        }

        public async Task<IEnumerable<Grn>> GetByStatusAsync(string status, int companyId)
        {
            return await _context.Grns
                .Where(g => g.Status == status &&
                            g.Warehouse.Branch.CompanyId == companyId &&
                            g.IsDeleted != true)
                .OrderByDescending(g => g.GrnId)
                .ToListAsync();
        }

        public async Task<bool> IsGrnNoUniqueAsync(string grnNo, int companyId, int? excludeId = null)
        {
            var query = _context.Grns
                .Where(g => g.GrnNo == grnNo && g.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(g => g.GrnId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task AddAsync(Grn entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.Grns.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Grn entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.Grns.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int grnId, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(grnId, companyId);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        public async Task UpdateStatusAsync(int grnId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            var entity = await _context.Grns.FindAsync(grnId);
            if (entity == null) return;

            entity.Status = status;
            if (workflowInstanceId.HasValue) entity.WorkflowInstanceId = workflowInstanceId;
            if (currentStageId.HasValue) entity.CurrentStageId = currentStageId;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBeDeletedAsync(int grnId, int companyId)
        {
            var entity = await GetByIdAsync(grnId, companyId);
            if (entity == null || entity.IsDeleted == true) return false;

            var hasReturn = await _context.PurchaseReturns
                .AnyAsync(r => r.GrnId == grnId && r.IsDeleted != true);

            return !hasReturn;
        }
    }
}