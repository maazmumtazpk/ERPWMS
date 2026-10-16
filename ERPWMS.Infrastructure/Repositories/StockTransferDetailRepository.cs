using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class StockTransferDetailRepository : IStockTransferDetailRepository
    {
        private readonly AppDbContext _context;

        public StockTransferDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockTransferDetail>> GetByTransferIdAsync(int transferId)
        {
            return await _context.StockTransferDetails
                .Include(std => std.Product)
                .Include(std => std.FromBin)
                .Include(std => std.ToBin)
                .Where(std => std.TransferId == transferId)
                .ToListAsync();
        }

        public async Task<StockTransferDetail?> GetByIdAsync(int detailId)
        {
            return await _context.StockTransferDetails
                .Include(std => std.Product)
                .Include(std => std.FromBin)
                .Include(std => std.ToBin)
                .FirstOrDefaultAsync(std => std.TransferDetailId == detailId);
        }

        public async Task AddAsync(StockTransferDetail entity)
        {
            await _context.StockTransferDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<StockTransferDetail> entities)
        {
            await _context.StockTransferDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockTransferDetail entity)
        {
            _context.StockTransferDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<StockTransferDetail> entities)
        {
            _context.StockTransferDetails.UpdateRange(entities);
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
    }
}

