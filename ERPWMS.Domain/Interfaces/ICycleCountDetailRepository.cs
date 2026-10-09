using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ICycleCountDetailRepository
    {
        Task<IEnumerable<CycleCountDetail>> GetByCycleCountIdAsync(int cycleCountId);
        Task<CycleCountDetail?> GetByIdAsync(int detailId);
        Task AddAsync(CycleCountDetail entity);
        Task AddRangeAsync(IEnumerable<CycleCountDetail> entities);
        Task UpdateAsync(CycleCountDetail entity);
        Task UpdateRangeAsync(IEnumerable<CycleCountDetail> entities);
        Task<bool> DeleteAsync(int detailId);
        Task<int> GetTotalVarianceAsync(int cycleCountId);
    }
}
