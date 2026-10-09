using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IGrnRepository
    {
        Task<IEnumerable<Grn>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<Grn?> GetByIdAsync(int grnId, int companyId);
        Task<Grn?> GetByGrnNoAsync(string grnNo, int companyId);
        Task<IEnumerable<Grn>> GetByPurchaseIdAsync(int purchaseId);
        Task<IEnumerable<Grn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<IEnumerable<Grn>> GetByStatusAsync(string status, int companyId);
        Task<bool> IsGrnNoUniqueAsync(string grnNo, int companyId, int? excludeId = null);
        Task AddAsync(Grn entity);
        Task UpdateAsync(Grn entity);
        Task SoftDeleteAsync(int grnId, int companyId, string deletedBy);
        Task UpdateStatusAsync(int grnId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task<bool> CanBeDeletedAsync(int grnId, int companyId);
    }
}



