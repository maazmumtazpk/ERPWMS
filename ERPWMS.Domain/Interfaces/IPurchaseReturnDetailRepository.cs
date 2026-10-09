using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IPurchaseReturnDetailRepository
    {
        Task<IEnumerable<PurchaseReturnDetail>> GetByReturnIdAsync(int returnId);
        Task<PurchaseReturnDetail?> GetByIdAsync(int returnDetailId);
        Task<IEnumerable<PurchaseReturnDetail>> GetByPurchaseDetailIdAsync(int purchaseDetailId);
        Task<IEnumerable<PurchaseReturnDetail>> GetByProductIdAsync(int productId);
        Task AddAsync(PurchaseReturnDetail entity);
        Task AddRangeAsync(IEnumerable<PurchaseReturnDetail> entities);
        Task UpdateAsync(PurchaseReturnDetail entity);
        Task UpdateRangeAsync(IEnumerable<PurchaseReturnDetail> entities);
        Task<bool> DeleteAsync(int returnDetailId);
        Task<int> GetTotalReturnedQtyByPurchaseDetailIdAsync(int purchaseDetailId);
    }
}
