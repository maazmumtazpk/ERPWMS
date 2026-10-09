using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IDispatchService
    {
        // Get all dispatches
        Task<IEnumerable<DispatchMaster>> GetAllAsync(int companyId, bool includeDeleted = false);

        // Get dispatch by ID
        Task<DispatchMaster?> GetByIdAsync(int dispatchId, int companyId);

        // Get by dispatch number
        Task<DispatchMaster?> GetByDispatchNoAsync(string dispatchNo, int companyId);

        // Get by sales order
        Task<IEnumerable<DispatchMaster>> GetBySalesIdAsync(int salesId);

        // Get by status
        Task<IEnumerable<DispatchMaster>> GetByStatusAsync(string status, int companyId);

        // Get by date range
        Task<IEnumerable<DispatchMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);

        // Create dispatch
        Task<DispatchMaster> CreateDispatchAsync(DispatchMaster dispatch, List<DispatchDetail> details, string createdBy);

        // Update dispatch
        Task UpdateDispatchAsync(DispatchMaster dispatch, List<DispatchDetail> details, string updatedBy);

        // Delete dispatch
        Task<bool> DeleteDispatchAsync(int dispatchId, int companyId, string deletedBy);

        // Dispatch order (mark as dispatched)  ✅ FIXED: int → string
        Task<bool> DispatchOrderAsync(int dispatchId, string dispatchedBy);

        // Update tracking information
        Task<bool> UpdateTrackingAsync(int dispatchId, string trackingNo);

        // Confirm delivery
        Task<bool> ConfirmDeliveryAsync(int dispatchId, DateTime deliveryDate);

        // Check if dispatch number is unique
        Task<bool> IsDispatchNoUniqueAsync(string dispatchNo, int companyId, int? excludeId = null);

        // Check if can be deleted
        Task<bool> CanBeDeletedAsync(int dispatchId, int companyId);

        // Update status
        Task UpdateDispatchStatusAsync(int dispatchId, string status, int? workflowInstanceId = null, int? currentStageId = null);

        // Generate invoice from dispatch
        Task<Invoice> GenerateInvoiceFromDispatchAsync(int dispatchId, string createdBy);
    }
}