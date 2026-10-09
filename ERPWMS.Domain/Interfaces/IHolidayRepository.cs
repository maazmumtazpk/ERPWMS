using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IHolidayRepository
    {
        Task<Holiday?> GetByIdAsync(int id);
        Task<Holiday?> GetByDateAsync(DateOnly date, int? branchId);
        Task<IEnumerable<Holiday>> GetAllAsync(int? year, int? branchId, bool includeInactive);
        Task AddAsync(Holiday holiday);
        void Update(Holiday holiday);
        Task SaveChangesAsync();
    }
}
