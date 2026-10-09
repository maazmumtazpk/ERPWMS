using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories.Inventory
{
    public class StockAdjustmentDetailRepository : IStockAdjustmentDetailRepository
    {
        private readonly AppDbContext _context;

        public StockAdjustmentDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockAdjustmentDetail>> GetByAdjustmentIdAsync(int adjustmentId)
        {
            return await _context.StockAdjustmentDetails
                .Include(sad => sad.Product)
                .Include(sad => sad.Bin)
                .Where(sad => sad.AdjustmentId == adjustmentId)
                .ToListAsync();
        }

        public async Task<StockAdjustmentDetail?> GetByIdAsync(int detailId)
        {
            return await _context.StockAdjustmentDetails
                .Include(sad => sad.Product)
                .Include(sad => sad.Bin)
                .FirstOrDefaultAsync(sad => sad.AdjustmentDetailId == detailId);
        }

        public async Task AddAsync(StockAdjustmentDetail entity)
        {
            await _context.StockAdjustmentDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<StockAdjustmentDetail> entities)
        {
            await _context.StockAdjustmentDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockAdjustmentDetail entity)
        {
            _context.StockAdjustmentDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<StockAdjustmentDetail> entities)
        {
            _context.StockAdjustmentDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int detailId)
        {
            var entity = await GetByIdAsync(detailId);
            if (entity == null) return false;
            _context.StockAdjustmentDetails.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
