using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IInventoryCycleCountService
    {
        Task<CycleCount> CreateCycleCountAsync(CycleCount cycleCount, List<CycleCountDetail> details, string createdBy);
        Task<CycleCount> UpdateCycleCountAsync(CycleCount cycleCount, List<CycleCountDetail> details, string updatedBy);
        Task<CycleCount?> GetCycleCountByIdAsync(int cycleCountId);
        Task<IEnumerable<CycleCount>> GetAllCycleCountsAsync(int warehouseId, bool includeDeleted = false);
        Task<bool> SubmitCycleCountAsync(int cycleCountId, int userId);
        Task<bool> VerifyCycleCountAsync(int cycleCountId, int userId);
        Task<bool> RejectCycleCountAsync(int cycleCountId, int userId);
        Task<bool> PostCycleCountAsync(int cycleCountId, int userId);
        Task<bool> DeleteCycleCountAsync(int cycleCountId, string deletedBy);
        Task<bool> CanBeVerifiedAsync(int cycleCountId);
        Task<bool> CanBePostedAsync(int cycleCountId);
        Task UpdateCycleCountStatusAsync(int cycleCountId, string status);
        Task<int> CalculateVarianceAsync(int cycleCountId);
        Task<StockAdjustment> CreateAdjustmentFromVarianceAsync(CycleCount cycleCount, int userId);
    }
}


