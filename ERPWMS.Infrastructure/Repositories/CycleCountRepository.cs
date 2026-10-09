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
    public class CycleCountRepository : ICycleCountRepository
    {
        private readonly AppDbContext _context;

        public CycleCountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CycleCount>> GetByWarehouseIdAsync(int warehouseId, bool includeDeleted = false)
        {
            var query = _context.CycleCounts
                .Include(cc => cc.Product)
                .Include(cc => cc.Bin)
                .Include(cc => cc.Warehouse)
                .Include(cc => cc.CountedByNavigation)
                
                .Where(cc => cc.WarehouseId == warehouseId);

            if (!includeDeleted)
                query = query.Where(cc => cc.IsDeleted != true);

            return await query.OrderByDescending(cc => cc.CycleCountId).ToListAsync();
        }

        public async Task<CycleCount?> GetByIdAsync(int cycleCountId)
        {
            return await _context.CycleCounts
                .Include(cc => cc.Product)
                .Include(cc => cc.Bin)
                .Include(cc => cc.Warehouse)
                .Include(cc => cc.CycleCountDetails)
                .Include(cc => cc.CountedByNavigation)
                
                .FirstOrDefaultAsync(cc => cc.CycleCountId == cycleCountId);
        }

        public async Task<IEnumerable<CycleCount>> GetByStatusAsync(string status)
        {
            return await _context.CycleCounts
                .Where(cc => cc.Status == status && cc.IsDeleted != true)
                .OrderByDescending(cc => cc.CycleCountId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CycleCount>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.CycleCounts.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(cc => cc.CreatedDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(cc => cc.CreatedDate <= toDate);

            return await query.OrderByDescending(cc => cc.CycleCountId).ToListAsync();
        }

        public async Task AddAsync(CycleCount entity)
        {
            await _context.CycleCounts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CycleCount entity)
        {
            _context.CycleCounts.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBeVerifiedAsync(int cycleCountId)
        {
            var cycleCount = await GetByIdAsync(cycleCountId);
            return cycleCount != null && cycleCount.Status == "Submitted" && cycleCount.IsDeleted != true;
        }

        public async Task<bool> CanBePostedAsync(int cycleCountId)
        {
            var cycleCount = await GetByIdAsync(cycleCountId);
            return cycleCount != null && cycleCount.Status == "Verified" && cycleCount.IsDeleted != true;
        }

        public async Task UpdateStatusAsync(int cycleCountId, string status)
        {
            var cycleCount = await _context.CycleCounts.FindAsync(cycleCountId);
            if (cycleCount != null)
            {
                cycleCount.Status = status;
                cycleCount.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task VerifyAsync(int cycleCountId, string verifiedBy, DateTime verifiedDate)
        {
            var cycleCount = await _context.CycleCounts.FindAsync(cycleCountId);
            if (cycleCount != null)
            {
                cycleCount.Status = "Verified";
                cycleCount.VerifiedBy = verifiedBy;
                cycleCount.VerifiedDate = verifiedDate;
                cycleCount.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task PostAsync(int cycleCountId, string postedBy)
        {
            var cycleCount = await _context.CycleCounts.FindAsync(cycleCountId);
            if (cycleCount != null)
            {
                cycleCount.Status = "Posted";
                cycleCount.UpdatedBy = postedBy;
                cycleCount.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectAsync(int cycleCountId, string rejectedBy)
        {
            var cycleCount = await _context.CycleCounts.FindAsync(cycleCountId);
            if (cycleCount != null)
            {
                cycleCount.Status = "Rejected";
                cycleCount.UpdatedBy = rejectedBy;
                cycleCount.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
