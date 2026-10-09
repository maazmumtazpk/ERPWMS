using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IInvoiceService
    {
        // Get all invoices
        Task<IEnumerable<Invoice>> GetAllAsync(int companyId, bool includeDeleted = false);

        // Get invoice by ID
        Task<Invoice?> GetByIdAsync(int invoiceId, int companyId);

        // Get by invoice number
        Task<Invoice?> GetByInvoiceNoAsync(string invoiceNo, int companyId);

        // Get by sales order
        Task<IEnumerable<Invoice>> GetBySalesIdAsync(int salesId);

        // Get by status
        Task<IEnumerable<Invoice>> GetByStatusAsync(string status, int companyId);

        // Get by date range
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);

        // Create invoice
        Task<Invoice> CreateInvoiceAsync(Invoice invoice, List<InvoiceDetail> details, string createdBy);

        // Update invoice
        Task UpdateInvoiceAsync(Invoice invoice, List<InvoiceDetail> details, string updatedBy);

        // Delete invoice
        Task<bool> DeleteInvoiceAsync(int invoiceId, int companyId, string deletedBy);

        // Post invoice
        Task<bool> PostInvoiceAsync(int invoiceId, int userId);

        // Record payment
        Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, int userId);

        // Check if invoice number is unique
        Task<bool> IsInvoiceNoUniqueAsync(string invoiceNo, int companyId, int? excludeId = null);

        // Check if can be deleted
        Task<bool> CanBeDeletedAsync(int invoiceId, int companyId);

        // Update status
        Task UpdateInvoiceStatusAsync(int invoiceId, string status, int? workflowInstanceId = null, int? currentStageId = null);

        // Calculate balance due
        Task<decimal> CalculateBalanceDueAsync(int invoiceId);

        // Print invoice (generate PDF/DTO)
        Task<Invoice> GetInvoiceWithDetailsAsync(int invoiceId, int companyId);
    }
}


