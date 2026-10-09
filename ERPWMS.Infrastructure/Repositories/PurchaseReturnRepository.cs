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
    public class PurchaseReturnRepository : IPurchaseReturnRepository
    {
        private readonly AppDbContext _context;

        public PurchaseReturnRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseReturn>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.PurchaseReturns
                .Include(r => r.Purchase)
                .Where(r => r.Purchase.Warehouse.Branch.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(r => r.IsDeleted != true);

            return await query.OrderByDescending(r => r.ReturnId).ToListAsync();
        }

        public async Task<PurchaseReturn?> GetByIdAsync(int returnId, int companyId)
        {
            return await _context.PurchaseReturns
                .Include(r => r.Purchase)
                .Include(r => r.PurchaseReturnDetails)
                .FirstOrDefaultAsync(r =>
                    r.ReturnId == returnId &&
                    r.Purchase.Warehouse.Branch.CompanyId == companyId);
        }

        public async Task<PurchaseReturn?> GetByReturnNoAsync(string returnNo, int companyId)
        {
            return await _context.PurchaseReturns
                .FirstOrDefaultAsync(r =>
                    r.ReturnNo == returnNo &&
                    r.Purchase.Warehouse.Branch.CompanyId == companyId &&
                    r.IsDeleted != true);
        }

        public async Task<IEnumerable<PurchaseReturn>> GetByPurchaseIdAsync(int purchaseId)
        {
            return await _context.PurchaseReturns
                .Where(r => r.PurchaseId == purchaseId && r.IsDeleted != true)
                .OrderByDescending(r => r.ReturnId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseReturn>> GetByGrnIdAsync(int grnId)
        {
            return await _context.PurchaseReturns
                .Where(r => r.GrnId == grnId && r.IsDeleted != true)
                .OrderByDescending(r => r.ReturnId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            var query = _context.PurchaseReturns
                .Where(r => r.Purchase.Warehouse.Branch.CompanyId == companyId && r.IsDeleted != true);

            if (fromDate.HasValue) query = query.Where(r => r.ReturnDate >= fromDate);
            if (toDate.HasValue) query = query.Where(r => r.ReturnDate <= toDate);

            return await query.OrderByDescending(r => r.ReturnDate).ToListAsync();
        }

        public async Task<IEnumerable<PurchaseReturn>> GetByStatusAsync(string status, int companyId)
        {
            return await _context.PurchaseReturns
                .Where(r => r.Status == status &&
                            r.Purchase.Warehouse.Branch.CompanyId == companyId &&
                            r.IsDeleted != true)
                .OrderByDescending(r => r.ReturnId)
                .ToListAsync();
        }

        public async Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null)
        {
            var query = _context.PurchaseReturns
                .Where(r => r.ReturnNo == returnNo && r.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(r => r.ReturnId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task AddAsync(PurchaseReturn entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.PurchaseReturns.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PurchaseReturn entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.PurchaseReturns.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int returnId, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(returnId, companyId);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        public async Task UpdateStatusAsync(int returnId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            var entity = await _context.PurchaseReturns.FindAsync(returnId);
            if (entity == null) return;

            entity.Status = status;
            if (workflowInstanceId.HasValue) entity.WorkflowInstanceId = workflowInstanceId;
            if (currentStageId.HasValue) entity.CurrentStageId = currentStageId;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBeDeletedAsync(int returnId, int companyId)
        {
            var entity = await GetByIdAsync(returnId, companyId);
            return entity != null && entity.IsDeleted != true;
        }

        public async Task ApproveAsync(int returnId, string approvedBy, DateTime approvedDate)
        {
            var entity = await _context.PurchaseReturns.FindAsync(returnId);
            if (entity == null) return;

            entity.Status = "Approved";
            entity.ApprovedBy = approvedBy;
            entity.ApprovedDate = approvedDate;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}