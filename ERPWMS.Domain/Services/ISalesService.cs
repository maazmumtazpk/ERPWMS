using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface ISalesService
    {
        // Get all sales orders
        Task<IEnumerable<SalesMaster>> GetAllAsync(int companyId, bool includeDeleted = false);

        // Get sales order by ID
        Task<SalesMaster?> GetByIdAsync(int salesId, int companyId);

        // Get by order number
        Task<SalesMaster?> GetByOrderNoAsync(string orderNo, int companyId);

        // Get by customer
        Task<IEnumerable<SalesMaster>> GetByCustomerIdAsync(int customerId, int companyId);

        // Get by date range
        Task<IEnumerable<SalesMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);

        // Get by status
        Task<IEnumerable<SalesMaster>> GetByStatusAsync(string status, int companyId);

        // Create sales order
        Task<SalesMaster> CreateSalesOrderAsync(SalesMaster sales, List<SalesDetail> details, string createdBy);

        // Update sales order
        Task UpdateSalesOrderAsync(SalesMaster sales, List<SalesDetail> details, string updatedBy);

        // Delete sales order (soft delete)
        Task<bool> DeleteSalesOrderAsync(int salesId, int companyId, string deletedBy);

        // Submit sales order (start workflow)
        Task<bool> SubmitSalesOrderAsync(int salesId, int companyId, int userId);

        // Approve sales order
        Task<bool> ApproveSalesOrderAsync(int salesId, int companyId, int userId);

        // Reject sales order
        Task<bool> RejectSalesOrderAsync(int salesId, int companyId, int userId);

        // Check if order number is unique
        Task<bool> IsOrderNoUniqueAsync(string orderNo, int companyId, int? excludeId = null);

        // Calculate total amount
        Task<decimal> CalculateTotalAmountAsync(List<SalesDetail> details);

        // Check if can be deleted
        Task<bool> CanBeDeletedAsync(int salesId, int companyId);

        // Update status
        Task UpdateSalesOrderStatusAsync(int salesId, string status, int? workflowInstanceId = null, int? currentStageId = null);

        // Generate picking list from sales order
        Task<PickingList> GeneratePickingListAsync(int salesId, int warehouseId, string createdBy);
    }
}


