using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories.Sales
{
    public class SalesDetailRepository : ISalesDetailRepository
    {
        private readonly AppDbContext _context;

        public SalesDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesDetail>> GetBySalesIdAsync(int salesId)
        {
            return await _context.SalesDetails
                .Include(sd => sd.Product)
                .Where(sd => sd.SalesId == salesId && sd.IsDeleted != true)
                .OrderBy(sd => sd.DetailId)
                .ToListAsync();
        }

        public async Task<SalesDetail?> GetByIdAsync(int detailId)
        {
            return await _context.SalesDetails
                .Include(sd => sd.Product)
                .Include(sd => sd.Sales)
                .FirstOrDefaultAsync(sd => sd.DetailId == detailId && sd.IsDeleted != true);
        }

        public async Task<IEnumerable<SalesDetail>> GetByProductIdAsync(int productId)
        {
            return await _context.SalesDetails
                .Include(sd => sd.Product)
                .Include(sd => sd.Sales)
                .Where(sd => sd.ProductId == productId && sd.IsDeleted != true)
                .OrderByDescending(sd => sd.DetailId)
                .ToListAsync();
        }

        public async Task AddAsync(SalesDetail entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.SalesDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<SalesDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            await _context.SalesDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SalesDetail entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.SalesDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<SalesDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _context.SalesDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteBySalesIdAsync(int salesId, string deletedBy)
        {
            var details = await _context.SalesDetails
                .Where(sd => sd.SalesId == salesId && sd.IsDeleted != true)
                .ToListAsync();

            foreach (var detail in details)
            {
                detail.IsDeleted = true;
                detail.DeletedBy = deletedBy;
                detail.DeletedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int detailId)
        {
            var entity = await GetByIdAsync(detailId);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalShippedQtyAsync(int salesId)
        {
            return await _context.SalesDetails
                .Where(sd => sd.SalesId == salesId && sd.IsDeleted != true)
                .SumAsync(sd => (decimal?)sd.ShippedQty) ?? 0;
        }

        public async Task<int> GetTotalPickedQtyBySalesDetailIdAsync(int salesDetailId)
        {
            return await _context.PickingDetails
                .Where(pd => pd.SalesDetailId == salesDetailId)
                .SumAsync(pd => (int?)pd.PickedQty) ?? 0;
        }
    }
}
