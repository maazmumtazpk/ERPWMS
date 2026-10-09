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
    public class ShiftRepository : IShiftRepository
    {
        private readonly AppDbContext _context;

        public ShiftRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Shift?> GetByIdAsync(int id)
        {
            return await _context.Shifts
                .FirstOrDefaultAsync(s => s.ShiftId == id);
        }

        public async Task<Shift?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Shifts
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(s => s.ShiftId == id);
        }

        public async Task<Shift?> GetByNameAsync(string shiftName)
        {
            return await _context.Shifts
                .FirstOrDefaultAsync(s => s.ShiftName == shiftName);
        }

        public async Task<IEnumerable<Shift>> GetAllAsync()
        {
            return await _context.Shifts
                .OrderBy(s => s.ShiftName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetActiveShiftsAsync()
        {
            return await _context.Shifts
                .Where(s => s.IsActive == true && s.IsDeleted != true)
                .OrderBy(s => s.ShiftName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> SearchShiftsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var term = searchTerm.Trim().ToLower();
            return await _context.Shifts
                .Where(s => s.IsDeleted != true &&
                    (s.ShiftName.ToLower().Contains(term) ||
                     s.StartTime.ToString().Contains(term) ||
                     s.EndTime.ToString().Contains(term)))
                .OrderBy(s => s.ShiftName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetPagedShiftsAsync(int page, int pageSize, string? searchTerm, bool includeDeleted)
        {
            var query = _context.Shifts.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(s => s.IsDeleted != true);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.ShiftName.ToLower().Contains(term) ||
                    s.StartTime.ToString().Contains(term) ||
                    s.EndTime.ToString().Contains(term));
            }

            return await query
                .OrderBy(s => s.ShiftName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm, bool includeDeleted)
        {
            var query = _context.Shifts.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(s => s.IsDeleted != true);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.ShiftName.ToLower().Contains(term) ||
                    s.StartTime.ToString().Contains(term) ||
                    s.EndTime.ToString().Contains(term));
            }

            return await query.CountAsync();
        }

        public async Task<bool> ShiftNameExistsAsync(string shiftName, int? excludeId = null)
        {
            var query = _context.Shifts
                .Where(s => s.ShiftName == shiftName && s.IsDeleted != true);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.ShiftId != excludeId);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ShiftExistsAsync(int id)
        {
            return await _context.Shifts
                .AnyAsync(s => s.ShiftId == id && s.IsDeleted != true);
        }

        public async Task<int> GetEmployeeCountByShiftIdAsync(int shiftId)
        {
            return await _context.Employees
                .CountAsync(e => e.ShiftId == shiftId && e.IsDeleted != true);
        }

        public async Task AddAsync(Shift shift)
        {
            await _context.Shifts.AddAsync(shift);
        }

        public void Update(Shift shift)
        {
            _context.Shifts.Update(shift);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

