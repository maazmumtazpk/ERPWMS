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
    public class SalesMasterRepository : ISalesMasterRepository
    {
        private readonly AppDbContext _context;

        public SalesMasterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesMaster>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.SalesMasters
                .Include(s => s.Customer)
                .Include(s => s.Warehouse)
                .Where(s => s.Warehouse.Branch.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(s => s.IsDeleted != true);

            return await query.OrderByDescending(s => s.SalesId).ToListAsync();
        }

        public async Task<SalesMaster?> GetByIdAsync(int salesId, int companyId)
        {
            return await _context.SalesMasters
                .Include(s => s.Customer)
                .Include(s => s.Warehouse)
                .Include(s => s.SalesDetails)
                .FirstOrDefaultAsync(s =>
                    s.SalesId == salesId &&
                    s.Warehouse.Branch.CompanyId == companyId);
        }

        public async Task<SalesMaster?> GetByOrderNoAsync(string salesOrderNo, int companyId)
        {
            return await _context.SalesMasters
                .FirstOrDefaultAsync(s =>
                    s.SalesOrderNo == salesOrderNo &&
                    s.Warehouse.Branch.CompanyId == companyId &&
                    s.IsDeleted != true);
        }

        public async Task<IEnumerable<SalesMaster>> GetByCustomerIdAsync(int customerId, int companyId)
        {
            return await _context.SalesMasters
                .Where(s => s.CustomerId == customerId &&
                            s.Warehouse.Branch.CompanyId == companyId &&
                            s.IsDeleted != true)
                .OrderByDescending(s => s.SalesId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            var query = _context.SalesMasters
                .Where(s => s.Warehouse.Branch.CompanyId == companyId && s.IsDeleted != true);

            if (fromDate.HasValue) query = query.Where(s => s.OrderDate >= fromDate);
            if (toDate.HasValue) query = query.Where(s => s.OrderDate <= toDate);

            return await query.OrderByDescending(s => s.OrderDate).ToListAsync();
        }

        public async Task<IEnumerable<SalesMaster>> GetByStatusAsync(string status, int companyId)
        {
            return await _context.SalesMasters
                .Where(s => s.Status == status &&
                            s.Warehouse.Branch.CompanyId == companyId &&
                            s.IsDeleted != true)
                .OrderByDescending(s => s.SalesId)
                .ToListAsync();
        }

        public async Task<bool> IsOrderNoUniqueAsync(string salesOrderNo, int companyId, int? excludeId = null)
        {
            var query = _context.SalesMasters
                .Where(s => s.SalesOrderNo == salesOrderNo && s.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(s => s.SalesId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task AddAsync(SalesMaster entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.SalesMasters.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SalesMaster entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.SalesMasters.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int salesId, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(salesId, companyId);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        public async Task<bool> CanBeDeletedAsync(int salesId, int companyId)
        {
            var entity = await GetByIdAsync(salesId, companyId);
            if (entity == null || entity.IsDeleted == true) return false;

            if (await _context.PickingLists.AnyAsync(p => p.SalesId == salesId && p.IsDeleted != true)) return false;
            if (await _context.DispatchMasters.AnyAsync(d => d.SalesId == salesId && d.IsDeleted != true)) return false;
            if (await _context.Invoices.AnyAsync(i => i.SalesId == salesId && i.IsDeleted != true)) return false;

            return true;
        }

        public async Task UpdateStatusAsync(int salesId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            var entity = await _context.SalesMasters.FindAsync(salesId);
            if (entity == null) return;

            entity.Status = status;
            if (workflowInstanceId.HasValue) entity.WorkflowInstanceId = workflowInstanceId;
            if (currentStageId.HasValue) entity.CurrentStageId = currentStageId;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}