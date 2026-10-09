using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IStockTransferDetailRepository
    {
        Task<IEnumerable<StockTransferDetail>> GetByTransferIdAsync(int transferId);
        Task<StockTransferDetail?> GetByIdAsync(int detailId);
        Task AddAsync(StockTransferDetail entity);
        Task AddRangeAsync(IEnumerable<StockTransferDetail> entities);
        Task UpdateAsync(StockTransferDetail entity);
        Task UpdateRangeAsync(IEnumerable<StockTransferDetail> entities);
        Task<bool> DeleteAsync(int detailId);
    }
}
