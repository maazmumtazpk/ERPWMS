using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class WeeklyOffRepository : IWeeklyOffRepository
    {
        private readonly AppDbContext _context;
        public WeeklyOffRepository(AppDbContext c) { _context = c; }

        public async Task<WeeklyOff?> GetActiveByEmployeeAsync(int employeeId)
            => await _context.WeeklyOffs
                .Where(w => w.EmployeeId == employeeId && w.IsActive == true && w.IsDeleted != true)
                .OrderByDescending(w => w.EffectiveFrom)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<WeeklyOff>> GetAllAsync(int? employeeId)
        {
            var q = _context.WeeklyOffs
                .Include(w => w.Employee)
                .Where(w => w.IsDeleted != true);

            if (employeeId.HasValue)
                q = q.Where(w => w.EmployeeId == employeeId.Value);

            return await q.OrderByDescending(w => w.EffectiveFrom).ToListAsync();
        }

        public async Task AddAsync(WeeklyOff w) => await _context.WeeklyOffs.AddAsync(w);
        public void Update(WeeklyOff w) => _context.WeeklyOffs.Update(w);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
