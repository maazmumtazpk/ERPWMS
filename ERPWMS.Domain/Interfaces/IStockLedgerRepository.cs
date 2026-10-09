using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IStockLedgerRepository
    {
        Task<IEnumerable<StockLedger>> GetFilteredAsync(
            int companyId,
            int? productId = null,
            int? warehouseId = null,
            int? binId = null,
            string? transactionType = null,
            string? referenceDocNo = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? batchNo = null);

        Task<StockLedger> AddAsync(StockLedger ledger);
        Task<IEnumerable<StockLedger>> GetByProductIdAsync(int productId, int warehouseId);
        Task<decimal> GetCurrentBalanceAsync(int productId, int warehouseId, int binId);
        Task<IEnumerable<StockLedger>> GetByReferenceAsync(string referenceDocNo, string referenceDocType);
        Task<bool> IsLedgerAppendOnlyAsync(); // Always true - for validation
    }
}
