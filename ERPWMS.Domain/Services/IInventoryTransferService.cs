using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IInventoryTransferService
    {
        Task<StockTransfer> CreateTransferAsync(StockTransfer transfer, List<StockTransferDetail> details, string createdBy);
        Task<StockTransfer> UpdateTransferAsync(StockTransfer transfer, List<StockTransferDetail> details, string updatedBy);
        Task<StockTransfer?> GetTransferByIdAsync(int transferId);
        Task<IEnumerable<StockTransfer>> GetAllTransfersAsync(int warehouseId, bool includeDeleted = false);
        Task<bool> SubmitTransferAsync(int transferId, int userId);
        Task<bool> ApproveTransferAsync(int transferId, int userId);
        Task<bool> RejectTransferAsync(int transferId, int userId);
        Task<bool> DispatchTransferAsync(int transferId, int userId);
        Task<bool> ReceiveTransferAsync(int transferId, int userId);
        Task<bool> DeleteTransferAsync(int transferId, string deletedBy);
        Task<bool> CanBeDispatchedAsync(int transferId);
        Task<bool> CanBeReceivedAsync(int transferId);
        Task UpdateTransferStatusAsync(int transferId, string status);
        Task<bool> HasSufficientStockAsync(int transferId);
        Task ValidateTransferAsync(StockTransfer transfer, List<StockTransferDetail> details);
        Task<StockLedger> CreateLedgerEntryForTransferOutAsync(StockTransfer transfer, int userId);
        Task<StockLedger> CreateLedgerEntryForTransferInAsync(StockTransfer transfer, int userId);
    }
}


