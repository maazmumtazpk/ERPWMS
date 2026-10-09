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
    public class EmployeeSalaryRepository : IEmployeeSalaryRepository
    {
        private readonly AppDbContext _context;

        public EmployeeSalaryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeSalary?> GetByIdAsync(int id)
        {
            return await _context.EmployeeSalaries
                .FirstOrDefaultAsync(s => s.SalaryId == id);
        }

        public async Task<EmployeeSalary?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.EmployeeSalaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.SalaryId == id);
        }

        public async Task<IEnumerable<EmployeeSalary>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId)
                .Include(s => s.Employee)
                .OrderByDescending(s => s.EffectiveDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeSalary>> GetActiveByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                .Include(s => s.Employee)
                .OrderByDescending(s => s.EffectiveDate)
                .ToListAsync();
        }

        public async Task<EmployeeSalary?> GetLatestByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        public async Task<EmployeeSalary?> GetActiveByEmployeeIdAndDateAsync(int employeeId, DateOnly date)
        {
            return await _context.EmployeeSalaries
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId
                    && s.IsActive == true
                    && s.EffectiveDate <= date);
        }

        public async Task<IEnumerable<EmployeeSalary>> GetPagedByEmployeeIdAsync(int employeeId, int page, int pageSize)
        {
            return await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId)
                .Include(s => s.Employee)
                .OrderByDescending(s => s.EffectiveDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeSalaries
                .CountAsync(s => s.EmployeeId == employeeId);
        }

        public async Task<bool> SalaryExistsForEmployeeAndDateAsync(int employeeId, DateOnly effectiveDate, int? excludeId = null)
        {
            var query = _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId && s.EffectiveDate == effectiveDate);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.SalaryId != excludeId);
            }

            return await query.AnyAsync();
        }

        public async Task<EmployeeSalary?> GetActiveSalaryByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeSalaries
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.IsActive == true);
        }

        // =========================================================
        // ?? NEW: Total count for all salary records (dashboard)
        // =========================================================
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.EmployeeSalaries.CountAsync();
        }

        public async Task AddAsync(EmployeeSalary salary)
        {
            await _context.EmployeeSalaries.AddAsync(salary);
        }

        public void Update(EmployeeSalary salary)
        {
            _context.EmployeeSalaries.Update(salary);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
