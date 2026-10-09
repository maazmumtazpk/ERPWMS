using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ISalesDetailRepository
    {
        Task<IEnumerable<SalesDetail>> GetBySalesIdAsync(int salesId);
        Task<SalesDetail?> GetByIdAsync(int detailId);
        Task<IEnumerable<SalesDetail>> GetByProductIdAsync(int productId);
        Task AddAsync(SalesDetail entity);
        Task AddRangeAsync(IEnumerable<SalesDetail> entities);
        Task UpdateAsync(SalesDetail entity);
        Task UpdateRangeAsync(IEnumerable<SalesDetail> entities);
        Task SoftDeleteBySalesIdAsync(int salesId, string deletedBy);
        Task<bool> DeleteAsync(int detailId);
        Task<decimal> GetTotalShippedQtyAsync(int salesId);
        Task<int> GetTotalPickedQtyBySalesDetailIdAsync(int salesDetailId);
    }
}



