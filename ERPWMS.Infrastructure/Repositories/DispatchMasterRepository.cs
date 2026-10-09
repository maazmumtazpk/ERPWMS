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
    public class DispatchMasterRepository : IDispatchMasterRepository
    {
        private readonly AppDbContext _context;

        public DispatchMasterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DispatchMaster>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.DispatchMasters
                .Include(d => d.Sales)
                .Include(d => d.Warehouse)
                .Where(d => d.Warehouse.Branch.CompanyId == companyId);

            if (!includeDeleted)
                query = query.Where(d => d.IsDeleted != true);

            return await query.OrderByDescending(d => d.DispatchId).ToListAsync();
        }

        public async Task<DispatchMaster?> GetByIdAsync(int dispatchId, int companyId)
        {
            return await _context.DispatchMasters
                .Include(d => d.Sales)
                .Include(d => d.Warehouse)
                .Include(d => d.DispatchDetails)
                .FirstOrDefaultAsync(d =>
                    d.DispatchId == dispatchId &&
                    d.Warehouse.Branch.CompanyId == companyId);
        }

        public async Task<DispatchMaster?> GetByDispatchNoAsync(string dispatchNo, int companyId)
        {
            return await _context.DispatchMasters
                .Include(d => d.Sales)
                .FirstOrDefaultAsync(d =>
                    d.DispatchNo == dispatchNo &&
                    d.Warehouse.Branch.CompanyId == companyId &&
                    d.IsDeleted != true);
        }

        public async Task<IEnumerable<DispatchMaster>> GetBySalesIdAsync(int salesId)
        {
            return await _context.DispatchMasters
                .Where(d => d.SalesId == salesId && d.IsDeleted != true)
                .OrderByDescending(d => d.DispatchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DispatchMaster>> GetByStatusAsync(string status, int companyId)
        {
            return await _context.DispatchMasters
                .Where(d => d.Status == status &&
                            d.Warehouse.Branch.CompanyId == companyId &&
                            d.IsDeleted != true)
                .OrderByDescending(d => d.DispatchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DispatchMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            var query = _context.DispatchMasters
                .Where(d => d.Warehouse.Branch.CompanyId == companyId && d.IsDeleted != true);

            if (fromDate.HasValue)
                query = query.Where(d => d.DispatchDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(d => d.DispatchDate <= toDate);

            return await query.OrderByDescending(d => d.DispatchDate).ToListAsync();
        }

        public async Task<bool> IsDispatchNoUniqueAsync(string dispatchNo, int companyId, int? excludeId = null)
        {
            var query = _context.DispatchMasters
                .Where(d => d.DispatchNo == dispatchNo && d.IsDeleted != true);

            if (excludeId.HasValue)
                query = query.Where(d => d.DispatchId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task AddAsync(DispatchMaster entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.DispatchMasters.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DispatchMaster entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.DispatchMasters.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int dispatchId, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(dispatchId, companyId);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        public async Task UpdateStatusAsync(int dispatchId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            var entity = await _context.DispatchMasters.FindAsync(dispatchId);
            if (entity == null) return;

            entity.Status = status;
            if (workflowInstanceId.HasValue) entity.WorkflowInstanceId = workflowInstanceId;
            if (currentStageId.HasValue) entity.CurrentStageId = currentStageId;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBeDeletedAsync(int dispatchId, int companyId)
        {
            var entity = await GetByIdAsync(dispatchId, companyId);
            if (entity == null || entity.IsDeleted == true) return false;

            var hasInvoice = await _context.Invoices
                .AnyAsync(i => i.SalesId == entity.SalesId && i.IsDeleted != true);

            return !hasInvoice;
        }
    }
}