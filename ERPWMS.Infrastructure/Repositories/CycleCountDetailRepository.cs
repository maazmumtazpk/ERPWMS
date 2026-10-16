using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories.Inventory
{
    public class CycleCountDetailRepository : ICycleCountDetailRepository
    {
        private readonly AppDbContext _context;

        public CycleCountDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CycleCountDetail>> GetByCycleCountIdAsync(int cycleCountId)
        {
            return await _context.CycleCountDetails
                .Include(ccd => ccd.Product)
                .Include(ccd => ccd.Bin)
                .Where(ccd => ccd.CountId == cycleCountId)
                .ToListAsync();
        }

        public async Task<CycleCountDetail?> GetByIdAsync(int detailId)
        {
            return await _context.CycleCountDetails
                .Include(ccd => ccd.Product)
                .Include(ccd => ccd.Bin)
                .FirstOrDefaultAsync(ccd => ccd.CountDetailId == detailId);
        }

        public async Task AddAsync(CycleCountDetail entity)
        {
            await _context.CycleCountDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<CycleCountDetail> entities)
        {
            await _context.CycleCountDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CycleCountDetail entity)
        {
            _context.CycleCountDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<CycleCountDetail> entities)
        {
            _context.CycleCountDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int detailId)
        {
            var entity = await GetByIdAsync(detailId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalVarianceAsync(int cycleCountId)
        {
            return await _context.CycleCountDetails
                .Where(ccd => ccd.CountId == cycleCountId)
                .SumAsync(ccd => ccd.Variance);
        }
    }
}

