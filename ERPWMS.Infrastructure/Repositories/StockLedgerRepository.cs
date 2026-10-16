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
        private const int MaxRows = 10000;
        private readonly AppDbContext _context;

        public StockLedgerRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET FILTERED (with safety limit + no tracking)
        // ============================================================
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
                .AsNoTracking()
                .Include(sl => sl.Product)
                .Include(sl => sl.Warehouse)
                    .ThenInclude(w => w.Branch)
                .Include(sl => sl.Bin)
                .Where(sl => sl.Warehouse.Branch.CompanyId == companyId);

            if (productId.HasValue)
                query = query.Where(sl => sl.ProductId == productId.Value);

            if (warehouseId.HasValue)
                query = query.Where(sl => sl.WarehouseId == warehouseId.Value);

            if (binId.HasValue)
                query = query.Where(sl => sl.BinId == binId.Value);

            if (!string.IsNullOrEmpty(transactionType))
                query = query.Where(sl => sl.TransactionType == transactionType);

            if (!string.IsNullOrEmpty(referenceDocNo))
                query = query.Where(sl => sl.ReferenceDocNo == referenceDocNo);

            if (fromDate.HasValue)
                query = query.Where(sl => sl.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(sl => sl.TransactionDate <= toDate.Value);

            if (!string.IsNullOrEmpty(batchNo))
                query = query.Where(sl => sl.BatchNo == batchNo);

            return await query
                .OrderByDescending(sl => sl.TransactionDate)
                .Take(MaxRows)
                .ToListAsync();
        }

        // ============================================================
        // ADD
        // ============================================================
        public async Task<StockLedger> AddAsync(StockLedger ledger)
        {
            if (ledger == null)
                throw new ArgumentNullException(nameof(ledger));

            ledger.CreatedDate = DateTime.UtcNow;

            await _context.StockLedgers.AddAsync(ledger);
            await _context.SaveChangesAsync();

            return ledger;
        }

        // ============================================================
        // GET BY PRODUCT + WAREHOUSE
        // ============================================================
        public async Task<IEnumerable<StockLedger>> GetByProductIdAsync(
            int productId,
            int warehouseId)
        {
            return await _context.StockLedgers
                .AsNoTracking()
                .Where(sl =>
                    sl.ProductId == productId &&
                    sl.WarehouseId == warehouseId &&
                    (sl.IsDeleted == null || sl.IsDeleted == false))
                .OrderByDescending(sl => sl.TransactionDate)
                .Take(MaxRows)
                .ToListAsync();
        }

        // ============================================================
        // GET CURRENT BALANCE (latest ledger row)
        // ============================================================
        public async Task<decimal> GetCurrentBalanceAsync(
            int productId,
            int warehouseId,
            int binId)
        {
            var ledger = await _context.StockLedgers
                .AsNoTracking()
                .Where(sl =>
                    sl.ProductId == productId &&
                    sl.WarehouseId == warehouseId &&
                    sl.BinId == binId &&
                    (sl.IsDeleted == null || sl.IsDeleted == false))
                .OrderByDescending(sl => sl.LedgerId)
                .FirstOrDefaultAsync();

            return ledger?.Balance ?? 0m;
        }

        // ============================================================
        // GET BY REFERENCE DOCUMENT
        // ============================================================
        public async Task<IEnumerable<StockLedger>> GetByReferenceAsync(
            string referenceDocNo,
            string referenceDocType)
        {
            if (string.IsNullOrWhiteSpace(referenceDocNo) || string.IsNullOrWhiteSpace(referenceDocType))
                return new List<StockLedger>();

            return await _context.StockLedgers
                .AsNoTracking()
                .Where(sl =>
                    sl.ReferenceDocNo == referenceDocNo &&
                    sl.ReferenceDocType == referenceDocType)
                .OrderByDescending(sl => sl.TransactionDate)
                .ToListAsync();
        }

        // ============================================================
        // LEDGER IS APPEND-ONLY
        // ============================================================
        public Task<bool> IsLedgerAppendOnlyAsync()
        {
            // Stock ledger is always append-only (no update/delete operations)
            return Task.FromResult(true);
        }
    }
}
