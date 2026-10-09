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
    public class StockLedgerRepository : IStockLedgerRepository
    {
        private readonly AppDbContext _context;

        public StockLedgerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockLedger>> GetFilteredAsync(
            int companyId,
            int? productId = null,
            int? warehouseId = null,
            int? binId = null,
            string? transactionType = null,
            string? referenceDocNo = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? batchNo = null)
        {
            var query = _context.StockLedgers
                .Include(sl => sl.Product)
                .Include(sl => sl.Warehouse)
                    .ThenInclude(w => w.Branch)
                .Include(sl => sl.Bin)
                .Where(sl => sl.Warehouse.Branch.CompanyId == companyId);

            if (productId.HasValue)
            {
                query = query.Where(sl =>
                    sl.ProductId == productId);
            }

            if (warehouseId.HasValue)
            {
                query = query.Where(sl =>
                    sl.WarehouseId == warehouseId);
            }

            if (binId.HasValue)
            {
                query = query.Where(sl =>
                    sl.BinId == binId);
            }

            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(sl =>
                    sl.TransactionType == transactionType);
            }

            if (!string.IsNullOrEmpty(referenceDocNo))
            {
                query = query.Where(sl =>
                    sl.ReferenceDocNo == referenceDocNo);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(sl =>
                    sl.TransactionDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(sl =>
                    sl.TransactionDate <= toDate);
            }

            if (!string.IsNullOrEmpty(batchNo))
            {
                query = query.Where(sl =>
                    sl.BatchNo == batchNo);
            }

            return await query
                .OrderByDescending(sl => sl.TransactionDate)
                .ToListAsync();
        }

        public async Task<StockLedger> AddAsync(StockLedger ledger)
        {
            ledger.CreatedDate = DateTime.UtcNow;

            await _context.StockLedgers.AddAsync(ledger);
            await _context.SaveChangesAsync();

            return ledger;
        }

        public async Task<IEnumerable<StockLedger>> GetByProductIdAsync(
            int productId,
            int warehouseId)
        {
            return await _context.StockLedgers
                .Where(sl =>
                    sl.ProductId == productId &&
                    sl.WarehouseId == warehouseId)
                .OrderByDescending(sl => sl.TransactionDate)
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentBalanceAsync(
            int productId,
            int warehouseId,
            int binId)
        {
            var ledger = await _context.StockLedgers
                .Where(sl =>
                    sl.ProductId == productId &&
                    sl.WarehouseId == warehouseId &&
                    sl.BinId == binId)
                .OrderByDescending(sl => sl.LedgerId)
                .FirstOrDefaultAsync();

            return ledger?.Balance ?? 0;
        }

        public async Task<IEnumerable<StockLedger>> GetByReferenceAsync(
            string referenceDocNo,
            string referenceDocType)
        {
            return await _context.StockLedgers
                .Where(sl =>
                    sl.ReferenceDocNo == referenceDocNo &&
                    sl.ReferenceDocType == referenceDocType)
                .OrderByDescending(sl => sl.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> IsLedgerAppendOnlyAsync()
        {
            // Stock ledger is always append-only
            return true;
        }
    }
}
