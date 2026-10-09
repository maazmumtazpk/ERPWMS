using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly AppDbContext _context;
        public HolidayRepository(AppDbContext context) { _context = context; }

        public async Task<Holiday?> GetByIdAsync(int id)
            => await _context.Holidays.FirstOrDefaultAsync(h => h.HolidayId == id);

        public async Task<Holiday?> GetByDateAsync(DateOnly date, int? branchId)
        {
            var q = _context.Holidays.Where(h => h.HolidayDate == date && h.IsDeleted != true && h.IsActive == true);
            if (branchId.HasValue)
                q = q.Where(h => h.BranchId == null || h.BranchId == branchId);
            return await q.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Holiday>> GetAllAsync(int? year, int? branchId, bool includeInactive)
        {
            var q = _context.Holidays.Where(h => h.IsDeleted != true);

            if (!includeInactive)
                q = q.Where(h => h.IsActive == true);

            if (year.HasValue)
                q = q.Where(h => h.HolidayDate.Year == year.Value);

            if (branchId.HasValue)
                q = q.Where(h => h.BranchId == null || h.BranchId == branchId);

            return await q.OrderBy(h => h.HolidayDate).ToListAsync();
        }

        public async Task AddAsync(Holiday h) => await _context.Holidays.AddAsync(h);
        public void Update(Holiday h) => _context.Holidays.Update(h);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
