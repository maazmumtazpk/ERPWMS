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
    public class SalesReturnRepository : ISalesReturnRepository
    {
        private readonly AppDbContext _context;

        public SalesReturnRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesReturn>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.SalesReturns
                .Include(r => r.Sales)
                .Where(r => r.Sales.Warehouse.Branch.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(r => r.IsDeleted != true);

            return await query.OrderByDescending(r => r.ReturnId).ToListAsync();
        }

        public async Task<SalesReturn?> GetByIdAsync(int returnId, int companyId)
        {
            return await _context.SalesReturns
                .Include(r => r.Sales)
                .Include(r => r.SalesReturnDetails)
                .FirstOrDefaultAsync(r =>
                    r.ReturnId == returnId &&
                    r.Sales.Warehouse.Branch.CompanyId == companyId);
        }

        public async Task<SalesReturn?> GetByReturnNoAsync(string returnNo, int companyId)
        {
            return await _context.SalesReturns
                .FirstOrDefaultAsync(r =>
                    r.ReturnNo == returnNo &&
                    r.Sales.Warehouse.Branch.CompanyId == companyId &&
                    r.IsDeleted != true);
        }

        public async Task<IEnumerable<SalesReturn>> GetBySalesIdAsync(int salesId)
        {
            return await _context.SalesReturns
                .Where(r => r.SalesId == salesId && r.IsDeleted != true)
                .OrderByDescending(r => r.ReturnId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesReturn>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _context.SalesReturns
                .Where(r => r.InvoiceId == invoiceId && r.IsDeleted != true)
                .OrderByDescending(r => r.ReturnId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesReturn>> GetByStatusAsync(string status, int companyId)
        {
            return await _context.SalesReturns
                .Where(r => r.Status == status &&
                            r.Sales.Warehouse.Branch.CompanyId == companyId &&
                            r.IsDeleted != true)
                .OrderByDescending(r => r.ReturnId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            var query = _context.SalesReturns
                .Where(r => r.Sales.Warehouse.Branch.CompanyId == companyId && r.IsDeleted != true);

            if (fromDate.HasValue) query = query.Where(r => r.ReturnDate >= fromDate);
            if (toDate.HasValue) query = query.Where(r => r.ReturnDate <= toDate);

            return await query.OrderByDescending(r => r.ReturnDate).ToListAsync();
        }

        public async Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null)
        {
            var query = _context.SalesReturns
                .Where(r => r.ReturnNo == returnNo && r.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(r => r.ReturnId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task AddAsync(SalesReturn entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.SalesReturns.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SalesReturn entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.SalesReturns.Update(entity);
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
            var entity = await _context.SalesReturns.FindAsync(returnId);
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
            var entity = await _context.SalesReturns.FindAsync(returnId);
            if (entity == null) return;

            entity.Status = "Approved";
            entity.ApprovedBy = approvedBy;
            entity.ApprovedDate = approvedDate;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}