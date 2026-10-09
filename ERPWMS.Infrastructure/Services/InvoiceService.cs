using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    // ? FIX: Primary Constructor
    public class InvoiceService(
        IInvoiceRepository invoiceRepository,
        IInvoiceDetailRepository invoiceDetailRepository) : IInvoiceService
    {
        public async Task<IEnumerable<Invoice>> GetAllAsync(int companyId, bool includeDeleted = false)
            => await invoiceRepository.GetAllAsync(companyId, includeDeleted);

        public async Task<Invoice?> GetByIdAsync(int invoiceId, int companyId)
            => await invoiceRepository.GetByIdAsync(invoiceId, companyId);

        public async Task<Invoice?> GetByInvoiceNoAsync(string invoiceNo, int companyId)
            => await invoiceRepository.GetByInvoiceNoAsync(invoiceNo, companyId);

        public async Task<IEnumerable<Invoice>> GetBySalesIdAsync(int salesId)
            => await invoiceRepository.GetBySalesIdAsync(salesId);

        public async Task<IEnumerable<Invoice>> GetByStatusAsync(string status, int companyId)
            => await invoiceRepository.GetByStatusAsync(status, companyId);

        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
            => await invoiceRepository.GetByDateRangeAsync(fromDate, toDate, companyId);

        public async Task<Invoice> CreateInvoiceAsync(
            Invoice invoice,
            List<InvoiceDetail> details,
            string createdBy)
        {
            if (!await invoiceRepository.IsInvoiceNoUniqueAsync(invoice.InvoiceNo, 0, null))
                throw new Exception("Invoice Number already exists.");

            invoice.Status = "Draft";
            invoice.CreatedBy = createdBy;   // ? already string
            invoice.CreatedDate = DateTime.UtcNow;

            await invoiceRepository.AddAsync(invoice);

            foreach (var detail in details)
            {
                detail.InvoiceId = invoice.InvoiceId;
                detail.CreatedBy = createdBy;   // ? already string
                detail.CreatedDate = DateTime.UtcNow;
                await invoiceDetailRepository.AddAsync(detail);
            }

            return invoice;
        }

        public async Task UpdateInvoiceAsync(
            Invoice invoice,
            List<InvoiceDetail> details,
            string updatedBy)
        {
            invoice.UpdatedBy = updatedBy;   // ? already string
            invoice.UpdatedDate = DateTime.UtcNow;

            await invoiceRepository.UpdateAsync(invoice);

            var existingDetails = await invoiceDetailRepository.GetByInvoiceIdAsync(invoice.InvoiceId);

            foreach (var existing in existingDetails)
            {
                await invoiceDetailRepository.DeleteAsync(existing.InvoiceDetailId);
            }

            foreach (var detail in details)
            {
                detail.InvoiceId = invoice.InvoiceId;
                detail.CreatedBy = updatedBy;   // ? already string
                detail.CreatedDate = DateTime.UtcNow;
                await invoiceDetailRepository.AddAsync(detail);
            }
        }

        public async Task<bool> DeleteInvoiceAsync(
            int invoiceId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(invoiceId, companyId))
                throw new Exception("Cannot delete Invoice as it may have associated returns.");

            await invoiceRepository.SoftDeleteAsync(invoiceId, companyId, deletedBy);
            return true;
        }

        public async Task<bool> PostInvoiceAsync(int invoiceId, int userId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, 0);
            if (invoice is null) return false;   // ? simplified

            invoice.Status = "Posted";
            invoice.UpdatedBy = userId.ToString();   // ? int -> string
            invoice.UpdatedDate = DateTime.UtcNow;

            await invoiceRepository.UpdateAsync(invoice);
            return true;
        }

        public async Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, int userId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, 0);
            if (invoice is null) return false;

            // ? FIX: BalanceDue nullable hai — safe calculation
            decimal currentBalance = (decimal?)invoice.BalanceDue ?? 0m;
            currentBalance -= amount;
            invoice.BalanceDue = currentBalance;

            invoice.UpdatedBy = userId.ToString();   // ? int -> string
            invoice.UpdatedDate = DateTime.UtcNow;

            if (currentBalance <= 0)
            {
                invoice.BalanceDue = 0;
                invoice.Status = "Paid";
            }

            await invoiceRepository.UpdateAsync(invoice);
            return true;
        }

        public async Task<bool> IsInvoiceNoUniqueAsync(string invoiceNo, int companyId, int? excludeId = null)
            => await invoiceRepository.IsInvoiceNoUniqueAsync(invoiceNo, companyId, excludeId);

        public async Task<bool> CanBeDeletedAsync(int invoiceId, int companyId)
            => await invoiceRepository.CanBeDeletedAsync(invoiceId, companyId);

        public async Task UpdateInvoiceStatusAsync(
            int invoiceId,
            string status,
            int? workflowInstanceId = null,
            int? currentStageId = null)
        {
            await invoiceRepository.UpdateStatusAsync(
                invoiceId, status, workflowInstanceId, currentStageId);
        }

        public async Task<decimal> CalculateBalanceDueAsync(int invoiceId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, 0);
            if (invoice is null) return 0m;

            // ? FIX: Nullable-safe return
            return (decimal?)invoice.BalanceDue ?? 0m;
        }

        public async Task<Invoice> GetInvoiceWithDetailsAsync(int invoiceId, int companyId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, companyId);
            if (invoice is null)
                throw new Exception("Invoice not found.");

            return invoice;
        }
    }
}