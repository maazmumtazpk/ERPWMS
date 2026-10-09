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
    public class PickingDetailRepository : IPickingDetailRepository
    {
        private readonly AppDbContext _context;

        public PickingDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PickingDetail>> GetByPickingIdAsync(int pickingId)
        {
            return await _context.PickingDetails
                .Include(pd => pd.Product)
                .Include(pd => pd.Bin)
                .Include(pd => pd.SalesDetail)
                .Where(pd => pd.PickingId == pickingId)
                .OrderBy(pd => pd.PickingDetailId)
                .ToListAsync();
        }

        public async Task<PickingDetail?> GetByIdAsync(int pickingDetailId)
        {
            return await _context.PickingDetails
                .Include(pd => pd.Product)
                .Include(pd => pd.Bin)
                .Include(pd => pd.SalesDetail)
                .Include(pd => pd.Picking)
                .FirstOrDefaultAsync(pd => pd.PickingDetailId == pickingDetailId);
        }

        public async Task<IEnumerable<PickingDetail>> GetBySalesDetailIdAsync(int salesDetailId)
        {
            return await _context.PickingDetails
                .Include(pd => pd.Product)
                .Include(pd => pd.Picking)
                .Where(pd => pd.SalesDetailId == salesDetailId)
                .OrderByDescending(pd => pd.PickingDetailId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PickingDetail>> GetByProductIdAsync(int productId)
        {
            return await _context.PickingDetails
                .Include(pd => pd.Product)
                .Include(pd => pd.Picking)
                .Where(pd => pd.ProductId == productId)
                .OrderByDescending(pd => pd.PickingDetailId)
                .ToListAsync();
        }

        public async Task AddAsync(PickingDetail entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.PickingDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<PickingDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            await _context.PickingDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PickingDetail entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.PickingDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<PickingDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _context.PickingDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int pickingDetailId)
        {
            var entity = await GetByIdAsync(pickingDetailId);
            if (entity == null) return false;
            entity.DeletedDate = DateTime.UtcNow;
            entity.DeletedBy = null; // You might want to pass a user ID
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalPickedQtyBySalesDetailIdAsync(int salesDetailId)
        {
            return await _context.PickingDetails
                .Where(pd => pd.SalesDetailId == salesDetailId)
                .SumAsync(pd => (int?)pd.PickedQty) ?? 0;
        }
    }
}

