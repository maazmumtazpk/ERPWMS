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
    public class DispatchDetailRepository : IDispatchDetailRepository
    {
        private readonly AppDbContext _context;

        public DispatchDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DispatchDetail>> GetByDispatchIdAsync(int dispatchId)
        {
            return await _context.DispatchDetails
                .Include(dd => dd.Product)
                .Include(dd => dd.SalesDetail)
                .Where(dd => dd.DispatchId == dispatchId && dd.IsDeleted != true)
                .OrderBy(dd => dd.DispatchDetailId)
                .ToListAsync();
        }

        public async Task<DispatchDetail?> GetByIdAsync(int dispatchDetailId)
        {
            return await _context.DispatchDetails
                .Include(dd => dd.Product)
                .Include(dd => dd.SalesDetail)
                .Include(dd => dd.Dispatch)
                .FirstOrDefaultAsync(dd => dd.DispatchDetailId == dispatchDetailId && dd.IsDeleted != true);
        }

        public async Task<IEnumerable<DispatchDetail>> GetBySalesDetailIdAsync(int salesDetailId)
        {
            return await _context.DispatchDetails
                .Include(dd => dd.Product)
                .Include(dd => dd.Dispatch)
                .Where(dd => dd.SalesDetailId == salesDetailId && dd.IsDeleted != true)
                .OrderByDescending(dd => dd.DispatchDetailId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DispatchDetail>> GetByProductIdAsync(int productId)
        {
            return await _context.DispatchDetails
                .Include(dd => dd.Product)
                .Include(dd => dd.Dispatch)
                .Where(dd => dd.ProductId == productId && dd.IsDeleted != true)
                .OrderByDescending(dd => dd.DispatchDetailId)
                .ToListAsync();
        }

        public async Task AddAsync(DispatchDetail entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.DispatchDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<DispatchDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            await _context.DispatchDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DispatchDetail entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.DispatchDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<DispatchDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _context.DispatchDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int dispatchDetailId)
        {
            var entity = await GetByIdAsync(dispatchDetailId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalShippedQtyBySalesDetailIdAsync(int salesDetailId)
        {
            return await _context.DispatchDetails
                .Where(dd => dd.SalesDetailId == salesDetailId && dd.IsDeleted != true)
                .SumAsync(dd => (int?)dd.Quantity) ?? 0;
        }
    }
}
