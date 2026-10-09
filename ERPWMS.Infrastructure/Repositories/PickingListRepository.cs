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
    public class PickingListRepository : IPickingListRepository
    {
        private readonly AppDbContext _context;

        public PickingListRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PickingList>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.PickingLists
                .Include(p => p.Sales)
                .Include(p => p.Warehouse)
                .Where(p => p.Warehouse.Branch.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(p => p.IsDeleted != true);

            return await query.OrderByDescending(p => p.PickingId).ToListAsync();
        }

        public async Task<PickingList?> GetByIdAsync(int pickingId, int companyId)
        {
            return await _context.PickingLists
                .Include(p => p.Sales)
                .Include(p => p.Warehouse)
                .Include(p => p.PickingDetails)
                .FirstOrDefaultAsync(p =>
                    p.PickingId == pickingId &&
                    p.Warehouse.Branch.CompanyId == companyId);
        }

        public async Task<PickingList?> GetByPickingNoAsync(string pickingNo, int companyId)
        {
            return await _context.PickingLists
                .Include(p => p.Sales)
                .FirstOrDefaultAsync(p =>
                    p.PickingNo == pickingNo &&
                    p.Warehouse.Branch.CompanyId == companyId &&
                    p.IsDeleted != true);
        }

        public async Task<IEnumerable<PickingList>> GetBySalesIdAsync(int salesId)
        {
            return await _context.PickingLists
                .Where(p => p.SalesId == salesId && p.IsDeleted != true)
                .OrderByDescending(p => p.PickingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PickingList>> GetByStatusAsync(string status, int companyId)
        {
            return await _context.PickingLists
                .Where(p => p.Status == status &&
                            p.Warehouse.Branch.CompanyId == companyId &&
                            p.IsDeleted != true)
                .OrderByDescending(p => p.PickingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PickingList>> GetByAssignedToAsync(int userId, int companyId)
        {
            return await _context.PickingLists
                .Where(p => p.AssignedTo == userId &&
                            p.Warehouse.Branch.CompanyId == companyId &&
                            p.IsDeleted != true)
                .OrderByDescending(p => p.PickingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PickingList>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            var query = _context.PickingLists
                .Where(p => p.Warehouse.Branch.CompanyId == companyId && p.IsDeleted != true);

            if (fromDate.HasValue) query = query.Where(p => p.CreatedDate >= fromDate);
            if (toDate.HasValue) query = query.Where(p => p.CreatedDate <= toDate);

            return await query.OrderByDescending(p => p.PickingId).ToListAsync();
        }

        public async Task<bool> IsPickingNoUniqueAsync(string pickingNo, int companyId, int? excludeId = null)
        {
            var query = _context.PickingLists
                .Where(p => p.PickingNo == pickingNo && p.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(p => p.PickingId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task AddAsync(PickingList entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.PickingLists.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PickingList entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.PickingLists.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int pickingId, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(pickingId, companyId);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        public async Task UpdateStatusAsync(int pickingId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            var entity = await _context.PickingLists.FindAsync(pickingId);
            if (entity == null) return;

            entity.Status = status;
            if (workflowInstanceId.HasValue) entity.WorkflowInstanceId = workflowInstanceId;
            if (currentStageId.HasValue) entity.CurrentStageId = currentStageId;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBeDeletedAsync(int pickingId, int companyId)
        {
            var entity = await GetByIdAsync(pickingId, companyId);
            if (entity == null || entity.IsDeleted == true) return false;

            var hasDispatch = await _context.DispatchMasters
                .AnyAsync(d => d.SalesId == entity.SalesId && d.IsDeleted != true);

            return !hasDispatch;
        }
    }
}