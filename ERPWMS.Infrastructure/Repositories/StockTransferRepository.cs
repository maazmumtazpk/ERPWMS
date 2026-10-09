using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories.Inventory
{
    public class StockTransferRepository : IStockTransferRepository
    {
        private readonly AppDbContext _context;

        public StockTransferRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockTransfer>> GetByFromWarehouseIdAsync(int warehouseId, bool includeDeleted = false)
        {
            var query = _context.StockTransfers
                .Include(st => st.Product)
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Where(st => st.FromWarehouseId == warehouseId);

            if (!includeDeleted)
                query = query.Where(st => st.IsDeleted != true);

            return await query.OrderByDescending(st => st.TransferId).ToListAsync();
        }

        public async Task<IEnumerable<StockTransfer>> GetByToWarehouseIdAsync(int warehouseId, bool includeDeleted = false)
        {
            var query = _context.StockTransfers
                .Include(st => st.Product)
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Where(st => st.ToWarehouseId == warehouseId);

            if (!includeDeleted)
                query = query.Where(st => st.IsDeleted != true);

            return await query.OrderByDescending(st => st.TransferId).ToListAsync();
        }

        public async Task<StockTransfer?> GetByIdAsync(int transferId)
        {
            return await _context.StockTransfers
                .Include(st => st.Product)
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Include(st => st.StockTransferDetails)
                .FirstOrDefaultAsync(st => st.TransferId == transferId);
        }

        public async Task<StockTransfer?> GetByTransferNoAsync(string transferNo)
        {
            return await _context.StockTransfers
                .FirstOrDefaultAsync(st => st.TransferNo == transferNo && st.IsDeleted != true);
        }

        public async Task<IEnumerable<StockTransfer>> GetByStatusAsync(string status)
        {
            return await _context.StockTransfers
                .Where(st => st.Status == status && st.IsDeleted != true)
                .OrderByDescending(st => st.TransferId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockTransfer>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.StockTransfers.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(st => st.CreatedDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(st => st.CreatedDate <= toDate);

            return await query.OrderByDescending(st => st.TransferId).ToListAsync();
        }

        public async Task<bool> IsTransferNoUniqueAsync(string transferNo, int? excludeId = null)
        {
            var query = _context.StockTransfers.Where(st => st.TransferNo == transferNo && st.IsDeleted != true);
            if (excludeId.HasValue)
                query = query.Where(st => st.TransferId != excludeId);
            return !await query.AnyAsync();
        }

        public async Task AddAsync(StockTransfer entity)
        {
            await _context.StockTransfers.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockTransfer entity)
        {
            _context.StockTransfers.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBeDispatchedAsync(int transferId)
        {
            var transfer = await GetByIdAsync(transferId);
            return transfer != null && transfer.Status == "Approved" && transfer.IsDeleted != true;
        }

        public async Task<bool> CanBeReceivedAsync(int transferId)
        {
            var transfer = await GetByIdAsync(transferId);
            return transfer != null && transfer.Status == "InTransit" && transfer.IsDeleted != true;
        }

        public async Task UpdateStatusAsync(int transferId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            var transfer = await _context.StockTransfers.FindAsync(transferId);
            if (transfer != null)
            {
                transfer.Status = status;
                if (workflowInstanceId.HasValue)
                    transfer.WorkflowInstanceId = workflowInstanceId;
                if (currentStageId.HasValue)
                    transfer.CurrentStageId = currentStageId;
                transfer.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApproveAsync(int transferId, string approvedBy, DateTime approvedDate)
        {
            var transfer = await _context.StockTransfers.FindAsync(transferId);
            if (transfer != null)
            {
                transfer.Status = "Approved";
                transfer.ApprovedBy = approvedBy;
                transfer.ApprovedDate = approvedDate;
                transfer.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        // ✅ FIXED: int dispatchedBy → string dispatchedBy
        public async Task DispatchAsync(int transferId, string dispatchedBy)
        {
            var transfer = await _context.StockTransfers.FindAsync(transferId);
            if (transfer != null)
            {
                transfer.Status = "InTransit";
                transfer.UpdatedBy = dispatchedBy;
                transfer.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReceiveAsync(int transferId, string receivedBy, DateTime receivedDate)
        {
            var transfer = await _context.StockTransfers.FindAsync(transferId);
            if (transfer != null)
            {
                transfer.Status = "Completed";
                transfer.ReceivedBy = receivedBy;
                transfer.ReceivedDate = receivedDate;
                transfer.UpdatedBy = receivedBy;
                transfer.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectAsync(int transferId, string rejectedBy)
        {
            var transfer = await _context.StockTransfers.FindAsync(transferId);
            if (transfer != null)
            {
                transfer.Status = "Rejected";
                transfer.UpdatedBy = rejectedBy;
                transfer.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasSufficientStockAsync(int transferId)
        {
            var transfer = await GetByIdAsync(transferId);
            if (transfer == null) return false;

            foreach (var detail in transfer.StockTransferDetails)
            {
                var balance = await _context.StockLedgers
                    .Where(sl => sl.ProductId == detail.ProductId &&
                                 sl.WarehouseId == transfer.FromWarehouseId &&
                                 sl.BinId == detail.FromBinId)
                    .OrderByDescending(sl => sl.LedgerId)
                    .Select(sl => sl.Balance)
                    .FirstOrDefaultAsync();

                if (balance < detail.Quantity)
                    return false;
            }
            return true;
        }
    }
}