using ERPWMS.Domain.Interfaces;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class PurchaseDetailRepository : IPurchaseDetailRepository
    {
        private readonly AppDbContext _context;

        public PurchaseDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET BY PURCHASE ID
        // =========================================================
        public async Task<IEnumerable<PurchaseDetail>> GetByPurchaseIdAsync(
            int purchaseId)
        {
            return await _context.PurchaseDetails
                .Where(x =>
                    x.PurchaseId == purchaseId &&
                    x.IsDeleted != true)
                .ToListAsync();
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<PurchaseDetail?> GetByIdAsync(int detailId)
        {
            return await _context.PurchaseDetails
                .FirstOrDefaultAsync(x =>
                    x.DetailId == detailId &&
                    x.IsDeleted != true);
        }

        // =========================================================
        // GET BY PRODUCT ID
        // =========================================================
        public async Task<IEnumerable<PurchaseDetail>> GetByProductIdAsync(
            int productId)
        {
            return await _context.PurchaseDetails
                .Where(x =>
                    x.ProductId == productId &&
                    x.IsDeleted != true)
                .ToListAsync();
        }

        // =========================================================
        // ADD
        // =========================================================
        public async Task AddAsync(PurchaseDetail entity)
        {
            await _context.PurchaseDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // ADD RANGE
        // =========================================================
        public async Task AddRangeAsync(
            IEnumerable<PurchaseDetail> entities)
        {
            await _context.PurchaseDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE
        // =========================================================
        public async Task UpdateAsync(PurchaseDetail entity)
        {
            _context.PurchaseDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE RANGE
        // =========================================================
        public async Task UpdateRangeAsync(
            IEnumerable<PurchaseDetail> entities)
        {
            _context.PurchaseDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // SOFT DELETE BY PURCHASE ID  ✅ FIXED (string deletedBy)
        // =========================================================
        public async Task SoftDeleteByPurchaseIdAsync(
            int purchaseId,
            string deletedBy)
        {
            var details = await _context.PurchaseDetails
                .Where(x =>
                    x.PurchaseId == purchaseId &&
                    x.IsDeleted != true)
                .ToListAsync();

            foreach (var detail in details)
            {
                detail.IsDeleted = true;
                detail.DeletedBy = deletedBy;
                detail.DeletedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // DELETE
        // =========================================================
        public async Task<bool> DeleteAsync(int detailId)
        {
            var entity = await _context.PurchaseDetails
                .FirstOrDefaultAsync(x =>
                    x.DetailId == detailId &&
                    x.IsDeleted != true);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // TOTAL RECEIVED QUANTITY
        // =========================================================
        public async Task<decimal> GetTotalReceivedQtyAsync(
            int purchaseId)
        {
            return await _context.PurchaseDetails
                .Where(x =>
                    x.PurchaseId == purchaseId &&
                    x.IsDeleted != true)
                .SumAsync(x => (decimal?)(x.ReceivedQty ?? 0)) ?? 0m;
        }
    }
}