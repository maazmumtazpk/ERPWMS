using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ISalesReturnDetailRepository
    {
        Task<IEnumerable<SalesReturnDetail>> GetByReturnIdAsync(int returnId);
        Task<SalesReturnDetail?> GetByIdAsync(int returnDetailId);
        Task<IEnumerable<SalesReturnDetail>> GetByInvoiceDetailIdAsync(int invoiceDetailId);
        Task<IEnumerable<SalesReturnDetail>> GetByProductIdAsync(int productId);
        Task AddAsync(SalesReturnDetail entity);
        Task AddRangeAsync(IEnumerable<SalesReturnDetail> entities);
        Task UpdateAsync(SalesReturnDetail entity);
        Task UpdateRangeAsync(IEnumerable<SalesReturnDetail> entities);
        Task<bool> DeleteAsync(int returnDetailId);
        Task<int> GetTotalReturnedQtyByInvoiceDetailIdAsync(int invoiceDetailId);
    }
}
