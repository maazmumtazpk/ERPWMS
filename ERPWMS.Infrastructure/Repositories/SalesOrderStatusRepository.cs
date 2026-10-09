using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories.Sales
{
    public class SalesOrderStatusRepository : ISalesOrderStatusRepository
    {
        private readonly AppDbContext _context;

        public SalesOrderStatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesOrderStatus>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.SalesOrderStatuses.AsQueryable();
            if (!includeInactive)
                query = query.Where(s => s.IsActive == true);
            return await query.ToListAsync();
        }

        public async Task<SalesOrderStatus?> GetByIdAsync(int statusId)
        {
            return await _context.SalesOrderStatuses.FindAsync(statusId);
        }

        public async Task<SalesOrderStatus?> GetByNameAsync(string statusName)
        {
            return await _context.SalesOrderStatuses
                .FirstOrDefaultAsync(s => s.StatusName == statusName);
        }

        public async Task<bool> IsActiveAsync(int statusId)
        {
            var status = await GetByIdAsync(statusId);
            return status != null && status.IsActive == true;
        }
    }
}
