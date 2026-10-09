using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IGrndetailRepository
    {
        Task<IEnumerable<Grndetail>> GetByGrnIdAsync(int grnId);
        Task<Grndetail?> GetByIdAsync(int grnDetailId);
        Task<IEnumerable<Grndetail>> GetByPurchaseDetailIdAsync(int purchaseDetailId);
        Task<IEnumerable<Grndetail>> GetByProductIdAsync(int productId);
        Task AddAsync(Grndetail entity);
        Task AddRangeAsync(IEnumerable<Grndetail> entities);
        Task UpdateAsync(Grndetail entity);
        Task UpdateRangeAsync(IEnumerable<Grndetail> entities);
        Task<bool> DeleteAsync(int grnDetailId);
        Task<int> GetTotalAcceptedQtyByPurchaseDetailIdAsync(int purchaseDetailId);
    }
}
