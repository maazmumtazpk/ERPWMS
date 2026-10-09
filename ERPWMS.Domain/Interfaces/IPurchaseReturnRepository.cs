using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IPurchaseReturnRepository
    {
        Task<IEnumerable<PurchaseReturn>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<PurchaseReturn?> GetByIdAsync(int returnId, int companyId);
        Task<PurchaseReturn?> GetByReturnNoAsync(string returnNo, int companyId);
        Task<IEnumerable<PurchaseReturn>> GetByPurchaseIdAsync(int purchaseId);
        Task<IEnumerable<PurchaseReturn>> GetByGrnIdAsync(int grnId);
        Task<IEnumerable<PurchaseReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<IEnumerable<PurchaseReturn>> GetByStatusAsync(string status, int companyId);
        Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null);
        Task AddAsync(PurchaseReturn entity);
        Task UpdateAsync(PurchaseReturn entity);
        Task SoftDeleteAsync(int returnId, int companyId, string deletedBy);
        Task UpdateStatusAsync(int returnId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task<bool> CanBeDeletedAsync(int returnId, int companyId);
        Task ApproveAsync(int returnId, string approvedBy, DateTime approvedDate);
    }
}



