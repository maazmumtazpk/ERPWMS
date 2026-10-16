using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class InvoiceService(
        IInvoiceRepository invoiceRepository,
        IInvoiceDetailRepository invoiceDetailRepository) : IInvoiceService
    {
        // =========================================================
        // QUERIES
        // =========================================================

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

        // =========================================================
        // CREATE
        // =========================================================

        public async Task<Invoice> CreateInvoiceAsync(
            Invoice invoice,
            List<InvoiceDetail> details,
            string createdBy)
        {
            if (invoice is null)
                throw new ArgumentNullException(nameof(invoice));

            if (details is null || !details.Any())
                throw new ArgumentException("Invoice must have at least one detail line.", nameof(details));

            if (!await invoiceRepository.IsInvoiceNoUniqueAsync(invoice.InvoiceNo, 0, null))
                throw new InvalidOperationException("Invoice Number already exists.");

            // Validate details
            foreach (var detail in details)
            {
                ValidateInvoiceDetail(detail);
            }

            invoice.Status = "Draft";
            invoice.CreatedBy = createdBy;
            invoice.CreatedDate = DateTime.UtcNow;

            await invoiceRepository.AddAsync(invoice);

            foreach (var detail in details)
            {
                detail.InvoiceId = invoice.InvoiceId;
                detail.CreatedBy = createdBy;
                detail.CreatedDate = DateTime.UtcNow;
                await invoiceDetailRepository.AddAsync(detail);
            }

            return invoice;
        }

        private static void ValidateInvoiceDetail(InvoiceDetail detail)
        {
            if (detail is null)
                throw new ArgumentException("Invoice detail cannot be null.");

            if (detail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (detail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateInvoiceAsync(
            Invoice invoice,
            List<InvoiceDetail> details,
            string updatedBy)
        {
            if (invoice is null)
                throw new ArgumentNullException(nameof(invoice));

            invoice.UpdatedBy = updatedBy;
            invoice.UpdatedDate = DateTime.UtcNow;

            await invoiceRepository.UpdateAsync(invoice);

            var existingDetails = await invoiceDetailRepository.GetByInvoiceIdAsync(invoice.InvoiceId);

            foreach (var existing in existingDetails)
            {
                await invoiceDetailRepository.DeleteAsync(existing.InvoiceDetailId);
            }

            if (details != null)
            {
                foreach (var detail in details)
                {
                    detail.InvoiceId = invoice.InvoiceId;
                    detail.CreatedBy = updatedBy;
                    detail.CreatedDate = DateTime.UtcNow;
                    await invoiceDetailRepository.AddAsync(detail);
                }
            }
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteInvoiceAsync(
            int invoiceId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(invoiceId, companyId))
                throw new InvalidOperationException("Cannot delete Invoice as it may have associated returns.");

            await invoiceRepository.SoftDeleteAsync(invoiceId, companyId, deletedBy);
            return true;
        }

        // =========================================================
        // POST
        // =========================================================

        public async Task<bool> PostInvoiceAsync(int invoiceId, int userId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, 0);
            if (invoice is null) return false;

            invoice.Status = "Posted";
            invoice.UpdatedBy = userId.ToString();
            invoice.UpdatedDate = DateTime.UtcNow;

            await invoiceRepository.UpdateAsync(invoice);
            return true;
        }

        // =========================================================
        // RECORD PAYMENT
        // =========================================================

        public async Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, int userId)
        {
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, 0);
            if (invoice is null) return false;

            decimal currentBalance = (decimal?)invoice.BalanceDue ?? 0m;

            if (amount > currentBalance)
                throw new ArgumentException(
                    $"Payment amount (${amount}) cannot exceed balance due (${currentBalance}).",
                    nameof(amount));

            currentBalance -= amount;
            invoice.BalanceDue = currentBalance;
            invoice.UpdatedBy = userId.ToString();
            invoice.UpdatedDate = DateTime.UtcNow;

            if (currentBalance <= 0)
            {
                invoice.BalanceDue = 0;
                invoice.Status = "Paid";
            }

            await invoiceRepository.UpdateAsync(invoice);
            return true;
        }

        // =========================================================
        // CHECKS
        // =========================================================

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
            await invoiceRepository.UpdateStatusAsync(invoiceId, status, workflowInstanceId, currentStageId);
        }

        public async Task<decimal> CalculateBalanceDueAsync(int invoiceId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, 0);
            if (invoice is null) return 0m;

            return (decimal?)invoice.BalanceDue ?? 0m;
        }

        public async Task<Invoice> GetInvoiceWithDetailsAsync(int invoiceId, int companyId)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, companyId);
            if (invoice is null)
                throw new InvalidOperationException("Invoice not found.");

            return invoice;
        }
    }
}

