using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IGrnService
    {
        Task<IEnumerable<Grn>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<Grn?> GetByIdAsync(int grnId, int companyId);
        Task<Grn> CreateGrnAsync(Grn grn, List<Grndetail> details, string createdBy);
        Task UpdateGrnAsync(Grn grn, List<Grndetail> details, string updatedBy);
        Task<bool> DeleteGrnAsync(int grnId, int companyId, string deletedBy);
        Task<bool> PostGrnAsync(int grnId, int companyId, int userId);
        Task<bool> RejectGrnAsync(int grnId, int companyId, int userId);
        Task<Grn?> GetByGrnNoAsync(string grnNo, int companyId);
        Task<IEnumerable<Grn>> GetByPurchaseIdAsync(int purchaseId);
        Task<IEnumerable<Grn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<IEnumerable<Grn>> GetByStatusAsync(string status, int companyId);
        Task<bool> IsGrnNoUniqueAsync(string grnNo, int companyId, int? excludeId = null);
        Task<bool> CanBeDeletedAsync(int grnId, int companyId);
        Task UpdateGrnStatusAsync(int grnId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task ValidateGrnAgainstPurchaseOrderAsync(int purchaseId, List<Grndetail> details);
        Task UpdateInventoryOnPostAsync(int grnId);
    }
}


