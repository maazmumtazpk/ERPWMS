using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IDispatchDetailRepository
    {
        Task<IEnumerable<DispatchDetail>> GetByDispatchIdAsync(int dispatchId);
        Task<DispatchDetail?> GetByIdAsync(int dispatchDetailId);
        Task<IEnumerable<DispatchDetail>> GetBySalesDetailIdAsync(int salesDetailId);
        Task<IEnumerable<DispatchDetail>> GetByProductIdAsync(int productId);
        Task AddAsync(DispatchDetail entity);
        Task AddRangeAsync(IEnumerable<DispatchDetail> entities);
        Task UpdateAsync(DispatchDetail entity);
        Task UpdateRangeAsync(IEnumerable<DispatchDetail> entities);
        Task<bool> DeleteAsync(int dispatchDetailId);
        Task<int> GetTotalShippedQtyBySalesDetailIdAsync(int salesDetailId);
    }
}

