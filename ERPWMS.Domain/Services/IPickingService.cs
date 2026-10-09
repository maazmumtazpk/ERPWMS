using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IPickingService
    {
        // Get all picking lists
        Task<IEnumerable<PickingList>> GetAllAsync(int companyId, bool includeDeleted = false);

        // Get picking list by ID
        Task<PickingList?> GetByIdAsync(int pickingId, int companyId);

        // Get by picking number
        Task<PickingList?> GetByPickingNoAsync(string pickingNo, int companyId);

        // Get by sales order
        Task<IEnumerable<PickingList>> GetBySalesIdAsync(int salesId);

        // Get by status
        Task<IEnumerable<PickingList>> GetByStatusAsync(string status, int companyId);

        // Get by assigned user
        Task<IEnumerable<PickingList>> GetByAssignedToAsync(int userId, int companyId);

        // Create picking list
        Task<PickingList> CreatePickingListAsync(PickingList picking, List<PickingDetail> details, string createdBy);

        // Update picking list
        Task UpdatePickingListAsync(PickingList picking, List<PickingDetail> details, string updatedBy);

        // Delete picking list
        Task<bool> DeletePickingListAsync(int pickingId, int companyId, string deletedBy);

        // Assign picker
        Task<bool> AssignPickerAsync(int pickingId, int assignedTo, string assignedBy);

        // Confirm picking (mark as picked)
        Task<bool> ConfirmPickingAsync(int pickingId, string pickedBy);

        // Verify picking
        Task<bool> VerifyPickingAsync(int pickingId, string verifiedBy);

        // Check if picking number is unique
        Task<bool> IsPickingNoUniqueAsync(string pickingNo, int companyId, int? excludeId = null);

        // Check if can be deleted
        Task<bool> CanBeDeletedAsync(int pickingId, int companyId);

        // Update status
        Task UpdatePickingStatusAsync(int pickingId, string status, int? workflowInstanceId = null, int? currentStageId = null);

        // Generate dispatch from picking
        Task<DispatchMaster> GenerateDispatchFromPickingAsync(int pickingId, string createdBy);
    }
}


