using ERPWMS.Domain.Interfaces;   // ? Typo fix: Intfaces ? Interfaces
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class PurchaseMasterRepository : IPurchaseMasterRepository
    {
        private readonly AppDbContext _context;

        public PurchaseMasterRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET ALL
        // ============================================================

        public async Task<IEnumerable<PurchaseMaster>> GetAllAsync(
            int companyId,
            bool includeDeleted = false)
        {
            var query = _context.PurchaseMasters
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                // ? CreatedByNavigation hata diya (string FK ab)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(p => p.IsDeleted != true);
            }

            return await query
                .OrderByDescending(p => p.PurchaseId)
                .ToListAsync();
        }

        // ============================================================
        // GET BY ID
        // ============================================================

        public async Task<PurchaseMaster?> GetByIdAsync(
            int purchaseId,
            int companyId)
        {
            return await _context.PurchaseMasters
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Include(p => p.PurchaseDetails)
                // ? CreatedByNavigation hata diya
                .Include(p => p.CurrentStage)
                .Include(p => p.WorkflowInstance)
                .FirstOrDefaultAsync(p =>
                    p.PurchaseId == purchaseId &&
                    p.IsDeleted != true);
        }

        // ============================================================
        // GET BY PURCHASE ORDER NUMBER
        // ============================================================

        public async Task<PurchaseMaster?> GetByPurchaseOrderNoAsync(
            string purchaseOrderNo,
            int companyId)
        {
            return await _context.PurchaseMasters
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p =>
                    p.PurchaseOrderNo == purchaseOrderNo &&
                    p.IsDeleted != true);
        }

        // ============================================================
        // GET BY SUPPLIER
        // ============================================================

        public async Task<IEnumerable<PurchaseMaster>> GetBySupplierIdAsync(
            int supplierId,
            int companyId)
        {
            return await _context.PurchaseMasters
                .Include(p => p.Supplier)
                .Where(p =>
                    p.SupplierId == supplierId &&
                    p.IsDeleted != true)
                .OrderByDescending(p => p.PurchaseId)
                .ToListAsync();
        }

        // ============================================================
        // GET BY DATE RANGE
        // ============================================================

        public async Task<IEnumerable<PurchaseMaster>> GetByDateRangeAsync(
            DateTime? fromDate,
            DateTime? toDate,
            int companyId)
        {
            var query = _context.PurchaseMasters
                .Include(p => p.Supplier)
                .Where(p => p.IsDeleted != true);

            if (fromDate.HasValue)
            {
                query = query.Where(p =>
                    p.OrderDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p =>
                    p.OrderDate <= toDate);
            }

            return await query
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();
        }

        // ============================================================
        // GET BY STATUS
        // ============================================================

        public async Task<IEnumerable<PurchaseMaster>> GetByStatusAsync(
            string status,
            int companyId)
        {
            return await _context.PurchaseMasters
                .Include(p => p.Supplier)
                .Where(p =>
                    p.Status == status &&
                    p.IsDeleted != true)
                .OrderByDescending(p => p.PurchaseId)
                .ToListAsync();
        }

        // ============================================================
        // PURCHASE ORDER NUMBER UNIQUE
        // ============================================================

        public async Task<bool> IsPurchaseOrderNoUniqueAsync(
            string purchaseOrderNo,
            int companyId,
            int? excludeId = null)
        {
            var query = _context.PurchaseMasters
                .Where(p =>
                    p.PurchaseOrderNo == purchaseOrderNo &&
                    p.IsDeleted != true);

            if (excludeId.HasValue)
            {
                query = query.Where(p =>
                    p.PurchaseId != excludeId);
            }

            return !await query.AnyAsync();
        }

        // ============================================================
        // ADD
        // ============================================================

        public async Task AddAsync(PurchaseMaster entity)
        {
            entity.CreatedDate = DateTime.UtcNow;

            await _context.PurchaseMasters.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // UPDATE
        // ============================================================

        public async Task UpdateAsync(PurchaseMaster entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _context.PurchaseMasters.Update(entity);

            await _context.SaveChangesAsync();
        }

        // ============================================================
        // SOFT DELETE  ? FIXED: int deletedBy ? string deletedBy
        // ============================================================

        public async Task SoftDeleteAsync(
            int purchaseId,
            int companyId,
            string deletedBy)   // ? string, interface ke mutabiq
        {
            var entity =
                await GetByIdAsync(
                    purchaseId,
                    companyId);

            if (entity == null)
            {
                return;
            }

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;   // ? string = string (chalega)
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        // ============================================================
        // CAN BE DELETED
        // ============================================================

        public async Task<bool> CanBeDeletedAsync(
            int purchaseId,
            int companyId)
        {
            var entity =
                await GetByIdAsync(
                    purchaseId,
                    companyId);

            if (entity == null ||
                entity.IsDeleted == true)
            {
                return false;
            }

            // Grn exists?
            var hasGrn =
                await _context.Grns
                    .AnyAsync(g =>
                        g.PurchaseId == purchaseId &&
                        g.IsDeleted != true);

            if (hasGrn)
            {
                return false;
            }

            // Purchase Return exists?
            var hasReturn =
                await _context.PurchaseReturns
                    .AnyAsync(r =>
                        r.PurchaseId == purchaseId &&
                        r.IsDeleted != true);

            if (hasReturn)
            {
                return false;
            }

            return true;
        }

        // ============================================================
        // UPDATE STATUS
        // ============================================================

        public async Task UpdateStatusAsync(
            int purchaseId,
            string status,
            int? workflowInstanceId = null,
            int? currentStageId = null)
        {
            var entity =
                await _context.PurchaseMasters
                    .FindAsync(purchaseId);

            if (entity == null)
            {
                return;
            }

            entity.Status = status;

            if (workflowInstanceId.HasValue)
            {
                entity.WorkflowInstanceId =
                    workflowInstanceId;
            }

            if (currentStageId.HasValue)
            {
                entity.CurrentStageId =
                    currentStageId;
            }

            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}