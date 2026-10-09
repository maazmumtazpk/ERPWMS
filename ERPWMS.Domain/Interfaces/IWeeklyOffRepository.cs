using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IWeeklyOffRepository
    {
        Task<WeeklyOff?> GetActiveByEmployeeAsync(int employeeId);
        Task<IEnumerable<WeeklyOff>> GetAllAsync(int? employeeId);
        Task AddAsync(WeeklyOff w);
        void Update(WeeklyOff w);
        Task SaveChangesAsync();
    }
}
