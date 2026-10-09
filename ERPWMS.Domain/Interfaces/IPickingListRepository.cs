using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IPickingListRepository
    {
        Task<IEnumerable<PickingList>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<PickingList?> GetByIdAsync(int pickingId, int companyId);
        Task<PickingList?> GetByPickingNoAsync(string pickingNo, int companyId);
        Task<IEnumerable<PickingList>> GetBySalesIdAsync(int salesId);
        Task<IEnumerable<PickingList>> GetByStatusAsync(string status, int companyId);
        Task<IEnumerable<PickingList>> GetByAssignedToAsync(int userId, int companyId);
        Task<IEnumerable<PickingList>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<bool> IsPickingNoUniqueAsync(string pickingNo, int companyId, int? excludeId = null);
        Task AddAsync(PickingList entity);
        Task UpdateAsync(PickingList entity);
        Task SoftDeleteAsync(int pickingId, int companyId, string deletedBy);
        Task UpdateStatusAsync(int pickingId, string status, int? workflowInstanceId = null, int? currentStageId = null);
        Task<bool> CanBeDeletedAsync(int pickingId, int companyId);
    }
}



