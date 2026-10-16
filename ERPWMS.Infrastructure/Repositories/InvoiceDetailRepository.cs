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
    public class InvoiceDetailRepository : IInvoiceDetailRepository
    {
        private readonly AppDbContext _context;

        public InvoiceDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceDetail>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
                .Include(id => id.Product)
                .Include(id => id.SalesDetail)
                .Where(id => id.InvoiceId == invoiceId && id.IsDeleted != true)
                .OrderBy(id => id.InvoiceDetailId)
                .ToListAsync();
        }

        public async Task<InvoiceDetail?> GetByIdAsync(int invoiceDetailId)
        {
            return await _context.InvoiceDetails
                .Include(id => id.Product)
                .Include(id => id.SalesDetail)
                .Include(id => id.Invoice)
                .FirstOrDefaultAsync(id => id.InvoiceDetailId == invoiceDetailId && id.IsDeleted != true);
        }

        public async Task<IEnumerable<InvoiceDetail>> GetBySalesDetailIdAsync(int salesDetailId)
        {
            return await _context.InvoiceDetails
                .Include(id => id.Product)
                .Include(id => id.Invoice)
                .Where(id => id.SalesDetailId == salesDetailId && id.IsDeleted != true)
                .OrderByDescending(id => id.InvoiceDetailId)
                .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceDetail>> GetByProductIdAsync(int productId)
        {
            return await _context.InvoiceDetails
                .Include(id => id.Product)
                .Include(id => id.Invoice)
                .Where(id => id.ProductId == productId && id.IsDeleted != true)
                .OrderByDescending(id => id.InvoiceDetailId)
                .ToListAsync();
        }

        public async Task AddAsync(InvoiceDetail entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.InvoiceDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<InvoiceDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            await _context.InvoiceDetails.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InvoiceDetail entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.InvoiceDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<InvoiceDetail> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _context.InvoiceDetails.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int invoiceDetailId)
        {
            var entity = await GetByIdAsync(invoiceDetailId);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

