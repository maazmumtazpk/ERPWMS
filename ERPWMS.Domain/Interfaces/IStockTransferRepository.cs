using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IStockTransferRepository
    {
        Task<IEnumerable<StockTransfer>> GetByFromWarehouseIdAsync(int warehouseId, bool includeDeleted = false);
        Task<IEnumerable<StockTransfer>> GetByToWarehouseIdAsync(int warehouseId, bool includeDeleted = false);
        Task<StockTransfer?> GetByIdAsync(int transferId);
        Task<StockTransfer?> GetByTransferNoAsync(string transferNo);
        Task<IEnumerable<StockTransfer>> GetByStatusAsync(string status);
        Task<IEnumerable<StockTransfer>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate);
        Task<bool> IsTransferNoUniqueAsync(string transferNo, int? excludeId = null);
        Task AddAsync(StockTransfer entity);
        Task UpdateAsync(StockTransfer entity);
        Task<bool> CanBeDispatchedAsync(int transferId);
        Task<bool> CanBeReceivedAsync(int transferId);
        Task UpdateStatusAsync(int transferId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task ApproveAsync(int transferId, string approvedBy, DateTime approvedDate);
        Task DispatchAsync(int transferId, string dispatchedBy);    // ✅ FIXED: int → string
        Task ReceiveAsync(int transferId, string receivedBy, DateTime receivedDate);
        Task RejectAsync(int transferId, string rejectedBy);
        Task<bool> HasSufficientStockAsync(int transferId);
    }
}