using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IPurchaseMasterService
    {
        Task<IEnumerable<PurchaseMaster>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<PurchaseMaster?> GetByIdAsync(int purchaseId, int companyId);
        Task<PurchaseMaster> CreatePurchaseOrderAsync(PurchaseMaster purchase, List<PurchaseDetail> details, string createdBy);
        Task UpdatePurchaseOrderAsync(PurchaseMaster purchase, List<PurchaseDetail> details, string updatedBy);
        Task<bool> DeletePurchaseOrderAsync(int purchaseId, int companyId, string deletedBy);
        Task<bool> SubmitPurchaseOrderAsync(int purchaseId, int companyId, int userId);
        Task<bool> ApprovePurchaseOrderAsync(int purchaseId, int companyId, int userId);
        Task<bool> RejectPurchaseOrderAsync(int purchaseId, int companyId, int userId);
        Task<PurchaseMaster?> GetByOrderNoAsync(string orderNo, int companyId);
        Task<IEnumerable<PurchaseMaster>> GetBySupplierIdAsync(int supplierId, int companyId);
        Task<IEnumerable<PurchaseMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<IEnumerable<PurchaseMaster>> GetByStatusAsync(string status, int companyId);
        Task<bool> IsOrderNoUniqueAsync(string orderNo, int companyId, int? excludeId = null);
        Task<decimal> CalculateTotalAmountAsync(List<PurchaseDetail> details);
        Task<bool> CanBeDeletedAsync(int purchaseId, int companyId);
        Task UpdatePurchaseOrderStatusAsync(int purchaseId, string status, int? workflowInstanceId = null, int? currentStageId = null);
    }
}


