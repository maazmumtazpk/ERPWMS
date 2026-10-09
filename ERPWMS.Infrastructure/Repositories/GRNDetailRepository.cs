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
    public class GrndetailRepository : IGrndetailRepository
    {
        private readonly AppDbContext _context;

        public GrndetailRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<Grndetail?> GetByIdAsync(int grnDetailId)
        {
            return await _context.Grndetails
                .FirstOrDefaultAsync(x =>
                    x.GrndetailId == grnDetailId);
        }

        // =========================================================
        // GET BY Grn ID
        // =========================================================

        public async Task<IEnumerable<Grndetail>> GetByGrnIdAsync(int grnId)
        {
            return await _context.Grndetails
                .Where(x => x.GrnId == grnId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY PURCHASE DETAIL ID
        // =========================================================

        public async Task<IEnumerable<Grndetail>> GetByPurchaseDetailIdAsync(
            int purchaseDetailId)
        {
            return await _context.Grndetails
                .Where(x => x.PurchaseDetailId == purchaseDetailId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY PRODUCT ID
        // =========================================================

        public async Task<IEnumerable<Grndetail>> GetByProductIdAsync(
            int productId)
        {
            return await _context.Grndetails
                .Where(x => x.ProductId == productId)
                .ToListAsync();
        }

        // =========================================================
        // ADD
        // =========================================================

        public async Task AddAsync(Grndetail detail)
        {
            await _context.Grndetails.AddAsync(detail);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // ADD RANGE
        // =========================================================

        public async Task AddRangeAsync(IEnumerable<Grndetail> details)
        {
            await _context.Grndetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateAsync(Grndetail detail)
        {
            _context.Grndetails.Update(detail);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE RANGE
        // =========================================================

        public async Task UpdateRangeAsync(IEnumerable<Grndetail> details)
        {
            _context.Grndetails.UpdateRange(details);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(int grnDetailId)
        {
            var detail = await _context.Grndetails
                .FirstOrDefaultAsync(x =>
                    x.GrndetailId == grnDetailId);

            if (detail == null)
                return false;

            _context.Grndetails.Remove(detail);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // TOTAL ACCEPTED QUANTITY
        // =========================================================

        public async Task<int> GetTotalAcceptedQtyByPurchaseDetailIdAsync(
            int purchaseDetailId)
        {
            return await _context.Grndetails
                .Where(x => x.PurchaseDetailId == purchaseDetailId)
                .SumAsync(x => x.AcceptedQty);
        }
    }
}
