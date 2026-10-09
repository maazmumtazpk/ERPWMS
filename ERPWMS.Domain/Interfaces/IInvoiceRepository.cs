using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<Invoice?> GetByIdAsync(int invoiceId, int companyId);
        Task<Invoice?> GetByInvoiceNoAsync(string invoiceNo, int companyId);
        Task<IEnumerable<Invoice>> GetBySalesIdAsync(int salesId);
        Task<IEnumerable<Invoice>> GetByStatusAsync(string status, int companyId);
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<bool> IsInvoiceNoUniqueAsync(string invoiceNo, int companyId, int? excludeId = null);
        Task AddAsync(Invoice entity);
        Task UpdateAsync(Invoice entity);
        Task SoftDeleteAsync(int invoiceId, int companyId, string deletedBy);
        Task UpdateStatusAsync(int invoiceId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task<bool> CanBeDeletedAsync(int invoiceId, int companyId);
        Task<decimal> GetTotalPaidAsync(int invoiceId);
        Task UpdateBalanceDueAsync(int invoiceId);
    }
}



