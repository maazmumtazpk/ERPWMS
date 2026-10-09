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
    public class SalesReturnDetailRepository : ISalesReturnDetailRepository
    {
        private readonly AppDbContext _context;

        public SalesReturnDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesReturnDetail>> GetByReturnIdAsync(int returnId)
        {
            return await _context.SalesReturnDetails
                .Include(rd => rd.Product)
                .Include(rd => rd.InvoiceDetail)
                .Where(rd => rd.ReturnId == returnId && rd.IsDeleted != true)
                .OrderBy(rd => rd.ReturnDetailId)
                .ToListAsync();
        }

        public async Task<SalesReturnDetail?> GetByIdAsync(int returnDetailId)
        {
            return await _context.SalesReturnDetails
                .Include(rd => rd.Product)
                .Include(rd => rd.InvoiceDetail)
                .Include(rd => rd.Return)
                .FirstOrDefaultAsync(rd => rd.ReturnDetailId == returnDetailId && rd.IsDeleted != true);
        }

        public async Task<IEnumerable<SalesReturnDetail>> GetByInvoiceDetailIdAsync(int invoiceDetailId)
        {
            return await _context.SalesReturnDetails
                .Include(rd => rd.Product)
                .Include(rd => rd.Return)
                .Where(rd => rd.InvoiceDetailId == invoiceDetailId && rd.IsDeleted != true)
                .OrderByDescending(rd => rd.ReturnDetailId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesReturnDetail>> GetByProductIdAsync(int productId)
        {
            return await _context.SalesReturnDetails
                .Include(rd => rd.Product)
                .Include(rd => rd.Return)
                .Where(rd => rd.ProductId == productId && rd.IsDeleted != true)
                .OrderByDescending(rd => rd.ReturnDetailId)
                .ToListAsync();
        }

        public async Task AddAsync(SalesReturnDetail entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.SalesReturnDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<SalesReturnDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            await _context.SalesReturnDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SalesReturnDetail entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.SalesReturnDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<SalesReturnDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _context.SalesReturnDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int returnDetailId)
        {
            var entity = await GetByIdAsync(returnDetailId);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalReturnedQtyByInvoiceDetailIdAsync(int invoiceDetailId)
        {
            return await _context.SalesReturnDetails
                .Where(rd => rd.InvoiceDetailId == invoiceDetailId && rd.IsDeleted != true)
                .SumAsync(rd => (int?)rd.Quantity) ?? 0;
        }
    }
}
