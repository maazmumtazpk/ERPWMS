using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IStockAdjustmentRepository
    {
        Task<IEnumerable<StockAdjustment>> GetByWarehouseIdAsync(int warehouseId, bool includeDeleted = false);
        Task<StockAdjustment?> GetByIdAsync(int adjustmentId);
        Task<StockAdjustment?> GetByWorkflowInstanceIdAsync(int workflowInstanceId);
        Task<IEnumerable<StockAdjustment>> GetByStatusAsync(string status);
        Task<IEnumerable<StockAdjustment>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate);
        Task AddAsync(StockAdjustment entity);
        Task UpdateAsync(StockAdjustment entity);
        Task<bool> CanBePostedAsync(int adjustmentId);
        Task<bool> CanBeApprovedAsync(int adjustmentId);
        Task UpdateStatusAsync(int adjustmentId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task ApproveAsync(int adjustmentId, string approvedBy, DateTime approvedDate);
        Task PostAsync(int adjustmentId, string postedBy);
        Task RejectAsync(int adjustmentId, string rejectedBy);
    }
}
