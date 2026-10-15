using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class SalesReturnService : ISalesReturnService
    {
        private readonly ISalesReturnRepository _salesReturnRepository;
        private readonly ISalesReturnDetailRepository _salesReturnDetailRepository;
        private readonly ISalesMasterRepository _salesMasterRepository;

        public SalesReturnService(
            ISalesReturnRepository salesReturnRepository,
            ISalesReturnDetailRepository salesReturnDetailRepository,
            ISalesMasterRepository salesMasterRepository)
        {
            _salesReturnRepository = salesReturnRepository;
            _salesReturnDetailRepository = salesReturnDetailRepository;
            _salesMasterRepository = salesMasterRepository;
        }

        public async Task<IEnumerable<SalesReturn>> GetAllAsync(int companyId, bool includeDeleted = false)
            => await _salesReturnRepository.GetAllAsync(companyId, includeDeleted);

        public async Task<SalesReturn?> GetByIdAsync(int returnId, int companyId)
            => await _salesReturnRepository.GetByIdAsync(returnId, companyId);

        public async Task<SalesReturn?> GetByReturnNoAsync(string returnNo, int companyId)
            => await _salesReturnRepository.GetByReturnNoAsync(returnNo, companyId);

        public async Task<IEnumerable<SalesReturn>> GetBySalesIdAsync(int salesId)
            => await _salesReturnRepository.GetBySalesIdAsync(salesId);

        public async Task<IEnumerable<SalesReturn>> GetByInvoiceIdAsync(int invoiceId)
            => await _salesReturnRepository.GetByInvoiceIdAsync(invoiceId);

        public async Task<IEnumerable<SalesReturn>> GetByStatusAsync(string status, int companyId)
            => await _salesReturnRepository.GetByStatusAsync(status, companyId);

        public async Task<IEnumerable<SalesReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
            => await _salesReturnRepository.GetByDateRangeAsync(fromDate, toDate, companyId);

        public async Task<SalesReturn> CreateSalesReturnAsync(
            SalesReturn salesReturn,
            List<SalesReturnDetail> details,
            string createdBy)
        {
            if (salesReturn is null)
                throw new ArgumentNullException(nameof(salesReturn));

            if (details is null || !details.Any())
                throw new ArgumentException("Sales return must have at least one detail line.", nameof(details));

            if (!await _salesReturnRepository.IsReturnNoUniqueAsync(salesReturn.ReturnNo, 0, null))
                throw new Exception("Sales Return Number already exists.");

            foreach (var detail in details)
            {
                ValidateSalesReturnDetail(detail);
            }

            salesReturn.Status = "Draft";
            salesReturn.CreatedBy = createdBy;
            salesReturn.CreatedDate = DateTime.UtcNow;

            await _salesReturnRepository.AddAsync(salesReturn);

            foreach (var detail in details)
            {
                detail.ReturnId = salesReturn.ReturnId;
                detail.CreatedBy = createdBy;
                detail.CreatedDate = DateTime.UtcNow;
                await _salesReturnDetailRepository.AddAsync(detail);
            }

            return salesReturn;
        }

        private static void ValidateSalesReturnDetail(SalesReturnDetail detail)
        {
            if (detail is null)
                throw new ArgumentException("Sales return detail cannot be null.");

            if (detail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (detail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");
        }

        public async Task UpdateSalesReturnAsync(
            SalesReturn salesReturn,
            List<SalesReturnDetail> details,
            string updatedBy)
        {
            if (salesReturn is null)
                throw new ArgumentNullException(nameof(salesReturn));

            salesReturn.UpdatedBy = updatedBy;
            salesReturn.UpdatedDate = DateTime.UtcNow;

            await _salesReturnRepository.UpdateAsync(salesReturn);

            var existingDetails = await _salesReturnDetailRepository.GetByReturnIdAsync(salesReturn.ReturnId);

            foreach (var existing in existingDetails)
            {
                await _salesReturnDetailRepository.DeleteAsync(existing.ReturnDetailId);
            }

            if (details != null)
            {
                foreach (var detail in details)
                {
                    detail.ReturnId = salesReturn.ReturnId;
                    detail.CreatedBy = updatedBy;
                    detail.CreatedDate = DateTime.UtcNow;
                    await _salesReturnDetailRepository.AddAsync(detail);
                }
            }
        }

        public async Task<bool> DeleteSalesReturnAsync(
            int returnId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(returnId, companyId))
                throw new Exception("Cannot delete Sales Return as it may be in use.");

            await _salesReturnRepository.SoftDeleteAsync(returnId, companyId, deletedBy);
            return true;
        }

        public async Task<bool> SubmitSalesReturnAsync(int returnId, int companyId, int userId)
        {
            var salesReturn = await _salesReturnRepository.GetByIdAsync(returnId, companyId);
            if (salesReturn == null) return false;

            salesReturn.Status = "Submitted";
            salesReturn.UpdatedBy = userId.ToString();
            salesReturn.UpdatedDate = DateTime.UtcNow;

            await _salesReturnRepository.UpdateAsync(salesReturn);
            return true;
        }

        public async Task<bool> ApproveSalesReturnAsync(int returnId, int companyId, int userId)
        {
            var salesReturn = await _salesReturnRepository.GetByIdAsync(returnId, companyId);
            if (salesReturn == null) return false;

            await _salesReturnRepository.ApproveAsync(returnId, userId.ToString(), DateTime.UtcNow);
            await UpdateInventoryOnApproveAsync(returnId);
            return true;
        }

        public async Task<bool> RejectSalesReturnAsync(int returnId, int companyId, int userId)
        {
            var salesReturn = await _salesReturnRepository.GetByIdAsync(returnId, companyId);
            if (salesReturn == null) return false;

            salesReturn.Status = "Rejected";
            salesReturn.UpdatedBy = userId.ToString();
            salesReturn.UpdatedDate = DateTime.UtcNow;

            await _salesReturnRepository.UpdateAsync(salesReturn);
            return true;
        }

        public async Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null)
            => await _salesReturnRepository.IsReturnNoUniqueAsync(returnNo, companyId, excludeId);

        public async Task<bool> CanBeDeletedAsync(int returnId, int companyId)
            => await _salesReturnRepository.CanBeDeletedAsync(returnId, companyId);

        public async Task UpdateSalesReturnStatusAsync(int returnId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            await _salesReturnRepository.UpdateStatusAsync(returnId, status, workflowInstanceId, currentStageId);
        }

        public async Task ValidateReturnAgainstInvoiceAsync(int invoiceId, List<SalesReturnDetail> details)
        {
            if (details is null || !details.Any())
                throw new ArgumentException("Return must have details.", nameof(details));

            var sales = await _salesMasterRepository.GetByIdAsync(invoiceId, 0);
            if (sales == null)
                throw new Exception("Sales Order not found.");

            foreach (var detail in details)
            {
                if (detail.Quantity <= 0)
                    throw new Exception($"Invalid return quantity for item {detail.ProductId}.");

                if (detail.UnitPrice < 0)
                    throw new Exception($"Invalid return price for item {detail.ProductId}.");
            }

            await Task.CompletedTask;
        }

        public async Task UpdateInventoryOnApproveAsync(int returnId)
        {
            // TODO: Implement inventory ledger updates
            // This method should update the stock ledger when a return is approved
            await Task.CompletedTask;
        }
    }
}
