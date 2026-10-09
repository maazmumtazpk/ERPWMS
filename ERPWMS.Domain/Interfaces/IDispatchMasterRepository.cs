using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IDispatchMasterRepository
    {
        Task<IEnumerable<DispatchMaster>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<DispatchMaster?> GetByIdAsync(int dispatchId, int companyId);
        Task<DispatchMaster?> GetByDispatchNoAsync(string dispatchNo, int companyId);
        Task<IEnumerable<DispatchMaster>> GetBySalesIdAsync(int salesId);
        Task<IEnumerable<DispatchMaster>> GetByStatusAsync(string status, int companyId);
        Task<IEnumerable<DispatchMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<bool> IsDispatchNoUniqueAsync(string dispatchNo, int companyId, int? excludeId = null);
        Task AddAsync(DispatchMaster entity);
        Task UpdateAsync(DispatchMaster entity);
        Task SoftDeleteAsync(int dispatchId, int companyId, string deletedBy);
        Task UpdateStatusAsync(int dispatchId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task<bool> CanBeDeletedAsync(int dispatchId, int companyId);
    }
}



