using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ICycleCountRepository
    {
        Task<IEnumerable<CycleCount>> GetByWarehouseIdAsync(int warehouseId, bool includeDeleted = false);
        Task<CycleCount?> GetByIdAsync(int cycleCountId);
        Task<IEnumerable<CycleCount>> GetByStatusAsync(string status);
        Task<IEnumerable<CycleCount>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate);
        Task AddAsync(CycleCount entity);
        Task UpdateAsync(CycleCount entity);
        Task<bool> CanBeVerifiedAsync(int cycleCountId);
        Task<bool> CanBePostedAsync(int cycleCountId);
        Task UpdateStatusAsync(int cycleCountId, string status);
        Task VerifyAsync(int cycleCountId, string verifiedBy, DateTime verifiedDate);
        Task PostAsync(int cycleCountId, string postedBy);
        Task RejectAsync(int cycleCountId, string rejectedBy);
    }
}
