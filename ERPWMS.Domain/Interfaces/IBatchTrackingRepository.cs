using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IBatchTrackingRepository
    {
        Task<IEnumerable<BatchTracking>> GetByProductIdAsync(int productId);
        Task<IEnumerable<BatchTracking>> GetByGrndetailIdAsync(int grnDetailId);
        Task<BatchTracking?> GetByBatchNoAsync(string batchNo, int productId);
        Task<IEnumerable<BatchTracking>> GetByExpiryDateRangeAsync(DateOnly? fromDate, DateOnly? toDate);
        Task<IEnumerable<BatchTracking>> GetAvailableBatchAsync(int productId);
        Task<bool> IsBatchNoExistsAsync(string batchNo, int productId);
        Task AddAsync(BatchTracking entity);
        Task AddRangeAsync(IEnumerable<BatchTracking> entities);
        Task UpdateAsync(BatchTracking entity);
        Task UpdateRangeAsync(IEnumerable<BatchTracking> entities);
        Task<bool> CanBeDeletedAsync(int batchId);
        Task ReduceRemainingQtyAsync(int batchId, int quantity);
        Task<IEnumerable<BatchTracking>> GetExpiredBatchesAsync();
        Task<IEnumerable<BatchTracking>> GetNearExpiryBatchesAsync(int daysThreshold);
    }
}
