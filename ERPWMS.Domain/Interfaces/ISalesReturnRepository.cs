using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ISalesReturnRepository
    {
        Task<IEnumerable<SalesReturn>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<SalesReturn?> GetByIdAsync(int returnId, int companyId);
        Task<SalesReturn?> GetByReturnNoAsync(string returnNo, int companyId);
        Task<IEnumerable<SalesReturn>> GetBySalesIdAsync(int salesId);
        Task<IEnumerable<SalesReturn>> GetByInvoiceIdAsync(int invoiceId);
        Task<IEnumerable<SalesReturn>> GetByStatusAsync(string status, int companyId);
        Task<IEnumerable<SalesReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null);
        Task AddAsync(SalesReturn entity);
        Task UpdateAsync(SalesReturn entity);
        Task SoftDeleteAsync(int returnId, int companyId, string deletedBy);
        Task UpdateStatusAsync(int returnId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task<bool> CanBeDeletedAsync(int returnId, int companyId);
        Task ApproveAsync(int returnId, string approvedBy, DateTime approvedDate);
    }
}



