using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IShiftRepository
    {
        Task<Shift?> GetByIdAsync(int id);
        Task<Shift?> GetByIdWithDetailsAsync(int id);
        Task<Shift?> GetByNameAsync(string shiftName);
        Task<IEnumerable<Shift>> GetAllAsync();
        Task<IEnumerable<Shift>> GetActiveShiftsAsync();
        Task<IEnumerable<Shift>> SearchShiftsAsync(string searchTerm);
        Task<IEnumerable<Shift>> GetPagedShiftsAsync(int page, int pageSize, string? searchTerm, bool includeDeleted);
        Task<int> GetTotalCountAsync(string? searchTerm, bool includeDeleted);
        Task<bool> ShiftNameExistsAsync(string shiftName, int? excludeId = null);
        Task<bool> ShiftExistsAsync(int id);
        Task<int> GetEmployeeCountByShiftIdAsync(int shiftId);
        Task AddAsync(Shift shift);
        void Update(Shift shift);
        Task SaveChangesAsync();
    }
}

