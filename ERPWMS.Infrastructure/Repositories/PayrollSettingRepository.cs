using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class PayrollSettingRepository : IPayrollSettingRepository
    {
        private readonly AppDbContext _context;

        public PayrollSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PayrollSetting?> GetSettingAsync()
        {
            return await _context.PayrollSettings
                .OrderByDescending(p => p.SettingId)
                .FirstOrDefaultAsync();
        }

        public async Task<PayrollSetting?> GetByIdAsync(int id)
        {
            return await _context.PayrollSettings
                .FirstOrDefaultAsync(p => p.SettingId == id);
        }

        public async Task AddAsync(PayrollSetting setting)
        {
            await _context.PayrollSettings.AddAsync(setting);
        }

        public void Update(PayrollSetting setting)
        {
            _context.PayrollSettings.Update(setting);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
