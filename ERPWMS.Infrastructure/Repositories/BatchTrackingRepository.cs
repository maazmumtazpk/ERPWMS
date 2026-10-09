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
    public class BatchTrackingRepository : IBatchTrackingRepository
    {
        private readonly AppDbContext _context;

        public BatchTrackingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BatchTracking>> GetByProductIdAsync(int productId)
        {
            return await _context.BatchTrackings
                .Include(bt => bt.Product)
                .Include(bt => bt.Grndetail)
                .Where(bt => bt.ProductId == productId)
                .OrderByDescending(bt => bt.BatchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BatchTracking>> GetByGrndetailIdAsync(int grnDetailId)
        {
            return await _context.BatchTrackings
                .Include(bt => bt.Product)
                .Where(bt => bt.GrndetailId == grnDetailId)
                .ToListAsync();
        }

        public async Task<BatchTracking?> GetByBatchNoAsync(string batchNo, int productId)
        {
            return await _context.BatchTrackings
                .Include(bt => bt.Product)
                .FirstOrDefaultAsync(bt => bt.BatchNo == batchNo && bt.ProductId == productId && bt.IsActive == true);
        }

        public async Task<IEnumerable<BatchTracking>> GetByExpiryDateRangeAsync(DateOnly? fromDate, DateOnly? toDate)
        {
            var query = _context.BatchTrackings.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(bt => bt.ExpiryDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(bt => bt.ExpiryDate <= toDate);

            return await query.OrderBy(bt => bt.ExpiryDate).ToListAsync();
        }

        public async Task<IEnumerable<BatchTracking>> GetAvailableBatchAsync(int productId)
        {
            return await _context.BatchTrackings
                .Where(bt => bt.ProductId == productId && bt.RemainingQty > 0 && bt.IsActive == true)
                .OrderBy(bt => bt.ExpiryDate) // FEFO
                .ToListAsync();
        }

        public async Task<bool> IsBatchNoExistsAsync(string batchNo, int productId)
        {
            return await _context.BatchTrackings
                .AnyAsync(bt => bt.BatchNo == batchNo && bt.ProductId == productId);
        }

        public async Task AddAsync(BatchTracking entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.BatchTrackings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<BatchTracking> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            await _context.BatchTrackings.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BatchTracking entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.BatchTrackings.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<BatchTracking> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _context.BatchTrackings.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanBeDeletedAsync(int batchId)
        {
            var batch = await GetByIdAsync(batchId);
            return batch != null && batch.RemainingQty == 0 && batch.IsActive == true;
        }

        public async Task ReduceRemainingQtyAsync(int batchId, int quantity)
        {
            var batch = await _context.BatchTrackings.FindAsync(batchId);
            if (batch != null)
            {
                if (batch.RemainingQty < quantity)
                    throw new Exception($"Insufficient batch quantity. Available: {batch.RemainingQty}, Requested: {quantity}");

                batch.RemainingQty -= quantity;
                if (batch.RemainingQty == 0)
                    batch.IsActive = false;

                batch.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BatchTracking>> GetExpiredBatchesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.BatchTrackings
                .Where(bt => bt.ExpiryDate < today && bt.IsActive == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<BatchTracking>> GetNearExpiryBatchesAsync(int daysThreshold)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var threshold = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysThreshold));
            return await _context.BatchTrackings
                .Where(bt => bt.ExpiryDate >= today && bt.ExpiryDate <= threshold && bt.IsActive == true)
                .OrderBy(bt => bt.ExpiryDate)
                .ToListAsync();
        }

        private async Task<BatchTracking?> GetByIdAsync(int batchId)
        {
            return await _context.BatchTrackings
                .FirstOrDefaultAsync(bt => bt.BatchId == batchId);
        }
    }
}
