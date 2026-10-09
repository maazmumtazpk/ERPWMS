using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IPurchaseReturnService
    {
        Task<IEnumerable<PurchaseReturn>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<PurchaseReturn?> GetByIdAsync(int returnId, int companyId);
        Task<PurchaseReturn> CreateReturnAsync(PurchaseReturn purchaseReturn, List<PurchaseReturnDetail> details, string createdBy);
        Task UpdateReturnAsync(PurchaseReturn purchaseReturn, List<PurchaseReturnDetail> details, string updatedBy);
        Task<bool> DeleteReturnAsync(int returnId, int companyId, string deletedBy);
        Task<bool> SubmitReturnAsync(int returnId, int companyId, int userId);
        Task<bool> ApproveReturnAsync(int returnId, int companyId, int userId);
        Task<bool> RejectReturnAsync(int returnId, int companyId, int userId);
        Task<PurchaseReturn?> GetByReturnNoAsync(string returnNo, int companyId);
        Task<IEnumerable<PurchaseReturn>> GetByPurchaseIdAsync(int purchaseId);
        Task<IEnumerable<PurchaseReturn>> GetByGrnIdAsync(int grnId);
        Task<IEnumerable<PurchaseReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<IEnumerable<PurchaseReturn>> GetByStatusAsync(string status, int companyId);
        Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null);
        Task<bool> CanBeDeletedAsync(int returnId, int companyId);
        Task UpdateReturnStatusAsync(int returnId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task ValidateReturnAgainstPurchaseAsync(int purchaseId, List<PurchaseReturnDetail> details);
        Task UpdateInventoryOnApproveAsync(int returnId);
    }
}


