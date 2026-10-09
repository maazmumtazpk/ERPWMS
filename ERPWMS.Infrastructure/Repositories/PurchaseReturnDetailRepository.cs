using ERPWMS.Domain.Interfaces;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class PurchaseReturnDetailRepository : IPurchaseReturnDetailRepository
    {
        private readonly AppDbContext _context;

        public PurchaseReturnDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<PurchaseReturnDetail?> GetByIdAsync(
            int returnDetailId)
        {
            return await _context.PurchaseReturnDetails
                .FirstOrDefaultAsync(x =>
                    x.ReturnDetailId == returnDetailId);
        }

        // =========================================================
        // GET BY RETURN ID
        // =========================================================

        public async Task<IEnumerable<PurchaseReturnDetail>> GetByReturnIdAsync(
            int returnId)
        {
            return await _context.PurchaseReturnDetails
                .Where(x => x.ReturnId == returnId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY PURCHASE DETAIL ID
        // =========================================================

        public async Task<IEnumerable<PurchaseReturnDetail>>
            GetByPurchaseDetailIdAsync(int purchaseDetailId)
        {
            return await _context.PurchaseReturnDetails
                .Where(x =>
                    x.PurchaseDetailId == purchaseDetailId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY PRODUCT ID
        // =========================================================

        public async Task<IEnumerable<PurchaseReturnDetail>>
            GetByProductIdAsync(int productId)
        {
            return await _context.PurchaseReturnDetails
                .Where(x =>
                    x.ProductId == productId)
                .ToListAsync();
        }

        // =========================================================
        // GET TOTAL RETURNED QTY BY PURCHASE DETAIL ID
        // =========================================================

        public async Task<int>
            GetTotalReturnedQtyByPurchaseDetailIdAsync(
                int purchaseDetailId)
        {
            return await _context.PurchaseReturnDetails
                .Where(x =>
                    x.PurchaseDetailId == purchaseDetailId)
                .SumAsync(x => x.Quantity);
        }

        // =========================================================
        // ADD
        // =========================================================

        public async Task AddAsync(
            PurchaseReturnDetail detail)
        {
            await _context.PurchaseReturnDetails
                .AddAsync(detail);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // ADD RANGE
        // =========================================================

        public async Task AddRangeAsync(
            IEnumerable<PurchaseReturnDetail> details)
        {
            await _context.PurchaseReturnDetails
                .AddRangeAsync(details);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateAsync(
            PurchaseReturnDetail detail)
        {
            _context.PurchaseReturnDetails
                .Update(detail);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE RANGE
        // =========================================================

        public async Task UpdateRangeAsync(
            IEnumerable<PurchaseReturnDetail> details)
        {
            _context.PurchaseReturnDetails
                .UpdateRange(details);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(
            int returnDetailId)
        {
            var detail =
                await _context.PurchaseReturnDetails
                    .FirstOrDefaultAsync(x =>
                        x.ReturnDetailId == returnDetailId);

            if (detail == null)
                return false;

            _context.PurchaseReturnDetails
                .Remove(detail);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // DELETE BY RETURN ID
        // =========================================================

        public async Task<bool> DeleteByReturnIdAsync(
            int returnId)
        {
            var details =
                await _context.PurchaseReturnDetails
                    .Where(x => x.ReturnId == returnId)
                    .ToListAsync();

            if (details.Count == 0)
                return false;

            _context.PurchaseReturnDetails
                .RemoveRange(details);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
