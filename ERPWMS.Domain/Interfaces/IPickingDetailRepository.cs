using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IPickingDetailRepository
    {
        Task<IEnumerable<PickingDetail>> GetByPickingIdAsync(int pickingId);
        Task<PickingDetail?> GetByIdAsync(int pickingDetailId);
        Task<IEnumerable<PickingDetail>> GetBySalesDetailIdAsync(int salesDetailId);
        Task<IEnumerable<PickingDetail>> GetByProductIdAsync(int productId);
        Task AddAsync(PickingDetail entity);
        Task AddRangeAsync(IEnumerable<PickingDetail> entities);
        Task UpdateAsync(PickingDetail entity);
        Task UpdateRangeAsync(IEnumerable<PickingDetail> entities);
        Task<bool> DeleteAsync(int pickingDetailId);
        Task<int> GetTotalPickedQtyBySalesDetailIdAsync(int salesDetailId);
    }
}
