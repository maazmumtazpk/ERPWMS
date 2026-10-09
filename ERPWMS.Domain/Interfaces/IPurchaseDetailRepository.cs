using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IPurchaseDetailRepository
    {
        Task<IEnumerable<PurchaseDetail>> GetByPurchaseIdAsync(int purchaseId);
        Task<PurchaseDetail?> GetByIdAsync(int detailId);
        Task<IEnumerable<PurchaseDetail>> GetByProductIdAsync(int productId);
        Task AddAsync(PurchaseDetail entity);
        Task AddRangeAsync(IEnumerable<PurchaseDetail> entities);
        Task UpdateAsync(PurchaseDetail entity);
        Task UpdateRangeAsync(IEnumerable<PurchaseDetail> entities);
        Task SoftDeleteByPurchaseIdAsync(int purchaseId, string deletedBy);
        Task<bool> DeleteAsync(int detailId);
        Task<decimal> GetTotalReceivedQtyAsync(int purchaseId);
    }
}



