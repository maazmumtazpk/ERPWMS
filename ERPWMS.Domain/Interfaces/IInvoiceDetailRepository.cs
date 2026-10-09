using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{

    public interface IInvoiceDetailRepository
    {
        Task<IEnumerable<InvoiceDetail>> GetByInvoiceIdAsync(int invoiceId);
        Task<InvoiceDetail?> GetByIdAsync(int invoiceDetailId);
        Task<IEnumerable<InvoiceDetail>> GetBySalesDetailIdAsync(int salesDetailId);
        Task<IEnumerable<InvoiceDetail>> GetByProductIdAsync(int productId);
        Task AddAsync(InvoiceDetail entity);
        Task AddRangeAsync(IEnumerable<InvoiceDetail> entities);
        Task UpdateAsync(InvoiceDetail entity);
        Task UpdateRangeAsync(IEnumerable<InvoiceDetail> entities);
        Task<bool> DeleteAsync(int invoiceDetailId);
    }
}
