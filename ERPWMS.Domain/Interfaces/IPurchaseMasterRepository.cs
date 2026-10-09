using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IPurchaseMasterRepository
    {
        Task<IEnumerable<PurchaseMaster>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<PurchaseMaster?> GetByIdAsync(int purchaseId, int companyId);
        Task<PurchaseMaster?> GetByPurchaseOrderNoAsync(string purchaseOrderNo, int companyId);
        Task<IEnumerable<PurchaseMaster>> GetBySupplierIdAsync(int supplierId, int companyId);
        Task<IEnumerable<PurchaseMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<IEnumerable<PurchaseMaster>> GetByStatusAsync(string status, int companyId);
        Task<bool> IsPurchaseOrderNoUniqueAsync(string purchaseOrderNo, int companyId, int? excludeId = null);
        Task AddAsync(PurchaseMaster entity);
        Task UpdateAsync(PurchaseMaster entity);
        Task SoftDeleteAsync(int purchaseId, int companyId, string deletedBy);
        Task<bool> CanBeDeletedAsync(int purchaseId, int companyId);
        Task UpdateStatusAsync(int purchaseId, string status, int? workflowInstanceId = null, int? currentStageId = null);
    }
}


