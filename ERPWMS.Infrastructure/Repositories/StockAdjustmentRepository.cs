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
    public class StockAdjustmentRepository : IStockAdjustmentRepository
    {
        private readonly AppDbContext _context;

        public StockAdjustmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockAdjustment>> GetByWarehouseIdAsync(int warehouseId, bool includeDeleted = false)
        {
            var query = _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Bin)
                .Include(sa => sa.Warehouse)
                
                .Where(sa => sa.WarehouseId == warehouseId);

            if (!includeDeleted)
                query = query.Where(sa => sa.IsDeleted != true);

            return await query.OrderByDescending(sa => sa.AdjustmentId).ToListAsync();
        }

        public async Task<StockAdjustment?> GetByIdAsync(int adjustmentId)
        {
            return await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Bin)
                .Include(sa => sa.Warehouse)
                .Include(sa => sa.StockAdjustmentDetails)
                
                .Include(sa => sa.CurrentStage)
                .Include(sa => sa.WorkflowInstance)
                .FirstOrDefaultAsync(sa => sa.AdjustmentId == adjustmentId);
        }

        public async Task<StockAdjustment?> GetByWorkflowInstanceIdAsync(int workflowInstanceId)
        {
            return await _context.StockAdjustments
                .FirstOrDefaultAsync(sa => sa.WorkflowInstanceId == workflowInstanceId);
        }

        public async Task<IEnumerable<StockAdjustment>> GetByStatusAsync(string status)
        {
            return await _context.StockAdjustments
                .Where(sa => sa.Status == status && sa.IsDeleted != true)
                .OrderByDescending(sa => sa.AdjustmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockAdjustment>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.StockAdjustments.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(sa => sa.CreatedDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(sa => sa.CreatedDate <= toDate);

            return await query.OrderByDescending(sa => sa.AdjustmentId).ToListAsync();
        }

        public async Task AddAsync(StockAdjustment entity)
        {
            await _context.StockAdjustments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockAdjustment entity)
        {
            _context.StockAdjustments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBePostedAsync(int adjustmentId)
        {
            var adjustment = await GetByIdAsync(adjustmentId);
            return adjustment != null && adjustment.Status == "Approved" && adjustment.IsDeleted != true;
        }

        public async Task<bool> CanBeApprovedAsync(int adjustmentId)
        {
            var adjustment = await GetByIdAsync(adjustmentId);
            return adjustment != null && adjustment.Status == "Submitted" && adjustment.IsDeleted != true;
        }

        public async Task UpdateStatusAsync(int adjustmentId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            var adjustment = await _context.StockAdjustments.FindAsync(adjustmentId);
            if (adjustment != null)
            {
                adjustment.Status = status;
                if (workflowInstanceId.HasValue)
                    adjustment.WorkflowInstanceId = workflowInstanceId;
                if (currentStageId.HasValue)
                    adjustment.CurrentStageId = currentStageId;
                adjustment.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApproveAsync(int adjustmentId, string approvedBy, DateTime approvedDate)
        {
            var adjustment = await _context.StockAdjustments.FindAsync(adjustmentId);
            if (adjustment != null)
            {
                adjustment.Status = "Approved";
                adjustment.ApprovedBy = approvedBy;
                adjustment.ApprovedDate = approvedDate;
                adjustment.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task PostAsync(int adjustmentId, string postedBy)
        {
            var adjustment = await _context.StockAdjustments.FindAsync(adjustmentId);
            if (adjustment != null)
            {
                adjustment.Status = "Posted";
                adjustment.UpdatedBy = postedBy;
                adjustment.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectAsync(int adjustmentId, string rejectedBy)
        {
            var adjustment = await _context.StockAdjustments.FindAsync(adjustmentId);
            if (adjustment != null)
            {
                adjustment.Status = "Rejected";
                adjustment.UpdatedBy = rejectedBy;
                adjustment.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
