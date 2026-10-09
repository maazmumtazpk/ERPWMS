using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class SerialNumberRepository : ISerialNumberRepository
    {
        private readonly AppDbContext _context;

        public SerialNumberRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<SerialNumber?> GetByIdAsync(int serialId)
        {
            return await _context.SerialNumbers
                .Include(sn => sn.Product)
                .Include(sn => sn.Grndetail)
                .Include(sn => sn.SalesDetail)
                .Include(sn => sn.Customer)
                .FirstOrDefaultAsync(sn => sn.SerialId == serialId);
        }

        // =========================================================
        // GET BY PRODUCT ID
        // =========================================================
        public async Task<IEnumerable<SerialNumber>> GetByProductIdAsync(int productId)
        {
            return await _context.SerialNumbers
                .Include(sn => sn.Product)
                .Include(sn => sn.Grndetail)
                .Include(sn => sn.SalesDetail)
                .Include(sn => sn.Customer)
                .Where(sn => sn.ProductId == productId)
                .OrderByDescending(sn => sn.SerialId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY Grn DETAIL ID
        // =========================================================
        public async Task<IEnumerable<SerialNumber>> GetByGrndetailIdAsync(int grnDetailId)
        {
            return await _context.SerialNumbers
                .Include(sn => sn.Product)
                .Include(sn => sn.SalesDetail)
                .Where(sn => sn.GrndetailId == grnDetailId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY SALES DETAIL ID
        // =========================================================
        public async Task<IEnumerable<SerialNumber>> GetBySalesDetailIdAsync(int salesDetailId)
        {
            return await _context.SerialNumbers
                .Include(sn => sn.Product)
                .Include(sn => sn.Customer)
                .Where(sn => sn.SalesDetailId == salesDetailId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY CUSTOMER ID
        // =========================================================
        public async Task<IEnumerable<SerialNumber>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.SerialNumbers
                .Include(sn => sn.Product)
                .Where(sn => sn.CustomerId == customerId)
                .ToListAsync();
        }

        // =========================================================
        // GET BY SERIAL NUMBER
        // =========================================================
        public async Task<SerialNumber?> GetBySerialNoAsync(string serialNo)
        {
            return await _context.SerialNumbers
                .Include(sn => sn.Product)
                .Include(sn => sn.Grndetail)
                .Include(sn => sn.SalesDetail)
                .Include(sn => sn.Customer)
                .FirstOrDefaultAsync(sn => sn.SerialNo == serialNo);
        }

        // =========================================================
        // GET BY STATUS
        // =========================================================
        public async Task<IEnumerable<SerialNumber>> GetByStatusAsync(string status)
        {
            return await _context.SerialNumbers
                .Include(sn => sn.Product)
                .Where(sn => sn.Status == status)
                .ToListAsync();
        }

        // =========================================================
        // GET AVAILABLE SERIAL NUMBERS
        // =========================================================
        public async Task<IEnumerable<SerialNumber>> GetAvailableAsync(int productId)
        {
            return await _context.SerialNumbers
                .Where(sn =>
                    sn.ProductId == productId &&
                    sn.Status == "IN")
                .ToListAsync();
        }

        // =========================================================
        // CHECK SERIAL NUMBER EXISTS
        // =========================================================
        public async Task<bool> IsSerialNoExistsAsync(string serialNo)
        {
            return await _context.SerialNumbers
                .AnyAsync(sn => sn.SerialNo == serialNo);
        }

        // =========================================================
        // ADD
        // =========================================================
        public async Task AddAsync(SerialNumber entity)
        {
            entity.CreatedDate = DateTime.UtcNow;

            await _context.SerialNumbers.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // ADD RANGE
        // =========================================================
        public async Task AddRangeAsync(IEnumerable<SerialNumber> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }

            await _context.SerialNumbers.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE
        // =========================================================
        public async Task UpdateAsync(SerialNumber entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _context.SerialNumbers.Update(entity);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE RANGE
        // =========================================================
        public async Task UpdateRangeAsync(IEnumerable<SerialNumber> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }

            _context.SerialNumbers.UpdateRange(entities);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // CAN BE DELETED
        // =========================================================
        public async Task<bool> CanBeDeletedAsync(int serialId)
        {
            var serial = await GetByIdAsync(serialId);

            return serial != null &&
                   serial.Status == "IN" &&
                   serial.SalesDetailId == null;
        }

        // =========================================================
        // UPDATE STATUS
        // =========================================================
        public async Task UpdateStatusAsync(int serialId, string status)
        {
            var serial = await _context.SerialNumbers
                .FindAsync(serialId);

            if (serial != null)
            {
                serial.Status = status;
                serial.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        // =========================================================
        // ASSIGN TO SALES DETAIL
        // =========================================================
        public async Task AssignToSalesDetailAsync(
            int serialId,
            int salesDetailId,
            int customerId,
            DateTime outDate)
        {
            var serial = await _context.SerialNumbers
                .FindAsync(serialId);

            if (serial != null)
            {
                serial.Status = "Sold";
                serial.SalesDetailId = salesDetailId;
                serial.CustomerId = customerId;
                serial.OutDate = outDate;
                serial.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }
    }
}
