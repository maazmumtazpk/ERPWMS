using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IStockAdjustmentDetailRepository
    {
        Task<IEnumerable<StockAdjustmentDetail>> GetByAdjustmentIdAsync(int adjustmentId);
        Task<StockAdjustmentDetail?> GetByIdAsync(int detailId);
        Task AddAsync(StockAdjustmentDetail entity);
        Task AddRangeAsync(IEnumerable<StockAdjustmentDetail> entities);
        Task UpdateAsync(StockAdjustmentDetail entity);
        Task UpdateRangeAsync(IEnumerable<StockAdjustmentDetail> entities);
        Task<bool> DeleteAsync(int detailId);
    }
}
