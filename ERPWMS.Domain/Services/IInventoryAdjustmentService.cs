using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IInventoryAdjustmentService
    {
        Task<StockAdjustment> CreateAdjustmentAsync(StockAdjustment adjustment, List<StockAdjustmentDetail> details, string createdBy);
        Task<StockAdjustment> UpdateAdjustmentAsync(StockAdjustment adjustment, List<StockAdjustmentDetail> details, string updatedBy);
        Task<StockAdjustment?> GetAdjustmentByIdAsync(int adjustmentId);
        Task<IEnumerable<StockAdjustment>> GetAllAdjustmentsAsync(int warehouseId, bool includeDeleted = false);
        Task<bool> SubmitAdjustmentAsync(int adjustmentId, int userId);
        Task<bool> ApproveAdjustmentAsync(int adjustmentId, int userId);
        Task<bool> RejectAdjustmentAsync(int adjustmentId, int userId);
        Task<bool> PostAdjustmentAsync(int adjustmentId, int userId);
        Task<bool> DeleteAdjustmentAsync(int adjustmentId, string deletedBy);
        Task<bool> CanBePostedAsync(int adjustmentId);
        Task<bool> CanBeApprovedAsync(int adjustmentId);
        Task UpdateAdjustmentStatusAsync(int adjustmentId, string status);
        Task ValidateStockAvailabilityAsync(int productId, int warehouseId, int binId, int quantity, string adjustmentType);
        Task<StockLedger> CreateLedgerEntryForAdjustmentAsync(StockAdjustment adjustment, int userId);
    }
}


