using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface ISalesReturnService
    {
        // Get all sales returns
        Task<IEnumerable<SalesReturn>> GetAllAsync(int companyId, bool includeDeleted = false);

        // Get sales return by ID
        Task<SalesReturn?> GetByIdAsync(int returnId, int companyId);

        // Get by return number
        Task<SalesReturn?> GetByReturnNoAsync(string returnNo, int companyId);

        // Get by sales order
        Task<IEnumerable<SalesReturn>> GetBySalesIdAsync(int salesId);

        // Get by invoice
        Task<IEnumerable<SalesReturn>> GetByInvoiceIdAsync(int invoiceId);

        // Get by status
        Task<IEnumerable<SalesReturn>> GetByStatusAsync(string status, int companyId);

        // Get by date range
        Task<IEnumerable<SalesReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);

        // Create sales return
        Task<SalesReturn> CreateSalesReturnAsync(SalesReturn salesReturn, List<SalesReturnDetail> details, string createdBy);

        // Update sales return
        Task UpdateSalesReturnAsync(SalesReturn salesReturn, List<SalesReturnDetail> details, string updatedBy);

        // Delete sales return
        Task<bool> DeleteSalesReturnAsync(int returnId, int companyId, string deletedBy);

        // Submit sales return (start workflow)
        Task<bool> SubmitSalesReturnAsync(int returnId, int companyId, int userId);

        // Approve sales return
        Task<bool> ApproveSalesReturnAsync(int returnId, int companyId, int userId);

        // Reject sales return
        Task<bool> RejectSalesReturnAsync(int returnId, int companyId, int userId);

        // Check if return number is unique
        Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null);

        // Check if can be deleted
        Task<bool> CanBeDeletedAsync(int returnId, int companyId);

        // Update status
        Task UpdateSalesReturnStatusAsync(int returnId, string status, int? workflowInstanceId = null, int? currentStageId = null);

        // Validate return against invoice
        Task ValidateReturnAgainstInvoiceAsync(int invoiceId, List<SalesReturnDetail> details);

        // Update inventory on approval
        Task UpdateInventoryOnApproveAsync(int returnId);
    }
}


