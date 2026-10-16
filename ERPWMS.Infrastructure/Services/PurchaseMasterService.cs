using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class PurchaseMasterService : IPurchaseMasterService
    {
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public PurchaseMasterService(
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository)
        {
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
        }

        public async Task<IEnumerable<PurchaseMaster>> GetAllAsync(int companyId, bool includeDeleted = false)
            => await _purchaseMasterRepository.GetAllAsync(companyId, includeDeleted);

        public async Task<PurchaseMaster?> GetByIdAsync(int purchaseId, int companyId)
            => await _purchaseMasterRepository.GetByIdAsync(purchaseId, companyId);

        public async Task<PurchaseMaster> CreatePurchaseOrderAsync(
            PurchaseMaster purchase,
            List<PurchaseDetail> details,
            string createdBy)
        {
            if (purchase is null)
                throw new ArgumentNullException(nameof(purchase));

            if (details is null || !details.Any())
                throw new ArgumentException("Purchase order must have at least one detail line.", nameof(details));

            if (!await _purchaseMasterRepository.IsPurchaseOrderNoUniqueAsync(purchase.PurchaseOrderNo, 0, null))
                throw new InvalidOperationException("Purchase Order Number already exists.");

            foreach (var detail in details)
            {
                ValidatePurchaseDetail(detail);
            }

            purchase.Status = "Draft";
            purchase.CreatedBy = createdBy;
            purchase.CreatedDate = DateTime.UtcNow;

            await _purchaseMasterRepository.AddAsync(purchase);

            foreach (var detail in details)
            {
                detail.PurchaseId = purchase.PurchaseId;
                detail.CreatedBy = createdBy;
                detail.CreatedDate = DateTime.UtcNow;
                await _purchaseDetailRepository.AddAsync(detail);
            }

            return purchase;
        }

        private static void ValidatePurchaseDetail(PurchaseDetail detail)
        {
            if (detail is null)
                throw new ArgumentException("Purchase detail cannot be null.");

            if (detail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (detail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");
        }

        public async Task UpdatePurchaseOrderAsync(
            PurchaseMaster purchase,
            List<PurchaseDetail> details,
            string updatedBy)
        {
            if (purchase is null)
                throw new ArgumentNullException(nameof(purchase));

            purchase.UpdatedBy = updatedBy;
            purchase.UpdatedDate = DateTime.UtcNow;

            await _purchaseMasterRepository.UpdateAsync(purchase);

            var existingDetails = await _purchaseDetailRepository.GetByPurchaseIdAsync(purchase.PurchaseId);

            foreach (var existing in existingDetails)
            {
                await _purchaseDetailRepository.DeleteAsync(existing.DetailId);
            }

            if (details != null)
            {
                foreach (var detail in details)
                {
                    detail.PurchaseId = purchase.PurchaseId;
                    detail.CreatedBy = updatedBy;
                    detail.CreatedDate = DateTime.UtcNow;
                    await _purchaseDetailRepository.AddAsync(detail);
                }
            }
        }

        public async Task<bool> DeletePurchaseOrderAsync(
            int purchaseId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(purchaseId, companyId))
                throw new InvalidOperationException("Cannot delete Purchase Order as it may have associated Grn or Returns.");

            await _purchaseMasterRepository.SoftDeleteAsync(purchaseId, companyId, deletedBy);
            return true;
        }

        public async Task<bool> SubmitPurchaseOrderAsync(int purchaseId, int companyId, int userId)
        {
            var purchase = await _purchaseMasterRepository.GetByIdAsync(purchaseId, companyId);
            if (purchase == null) return false;

            purchase.Status = "Submitted";
            purchase.UpdatedBy = userId.ToString();
            purchase.UpdatedDate = DateTime.UtcNow;

            await _purchaseMasterRepository.UpdateAsync(purchase);
            return true;
        }

        public async Task<bool> ApprovePurchaseOrderAsync(int purchaseId, int companyId, int userId)
        {
            var purchase = await _purchaseMasterRepository.GetByIdAsync(purchaseId, companyId);
            if (purchase == null) return false;

            purchase.Status = "Approved";
            purchase.UpdatedBy = userId.ToString();
            purchase.UpdatedDate = DateTime.UtcNow;

            await _purchaseMasterRepository.UpdateAsync(purchase);
            return true;
        }

        public async Task<bool> RejectPurchaseOrderAsync(int purchaseId, int companyId, int userId)
        {
            var purchase = await _purchaseMasterRepository.GetByIdAsync(purchaseId, companyId);
            if (purchase == null) return false;

            purchase.Status = "Rejected";
            purchase.UpdatedBy = userId.ToString();
            purchase.UpdatedDate = DateTime.UtcNow;

            await _purchaseMasterRepository.UpdateAsync(purchase);
            return true;
        }

        public async Task<PurchaseMaster?> GetByOrderNoAsync(string orderNo, int companyId)
            => await _purchaseMasterRepository.GetByPurchaseOrderNoAsync(orderNo, companyId);

        public async Task<IEnumerable<PurchaseMaster>> GetBySupplierIdAsync(int supplierId, int companyId)
            => await _purchaseMasterRepository.GetBySupplierIdAsync(supplierId, companyId);

        public async Task<IEnumerable<PurchaseMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
            => await _purchaseMasterRepository.GetByDateRangeAsync(fromDate, toDate, companyId);

        public async Task<IEnumerable<PurchaseMaster>> GetByStatusAsync(string status, int companyId)
            => await _purchaseMasterRepository.GetByStatusAsync(status, companyId);

        public async Task<bool> IsOrderNoUniqueAsync(string orderNo, int companyId, int? excludeId = null)
            => await _purchaseMasterRepository.IsPurchaseOrderNoUniqueAsync(orderNo, companyId, excludeId);

        public Task<decimal> CalculateTotalAmountAsync(List<PurchaseDetail> details)
        {
            if (details is null || !details.Any())
                return Task.FromResult(0m);

            decimal total = 0;
            foreach (var d in details)
            {
                total += (d.Quantity * d.UnitPrice) - (d.DiscountAmount ?? 0);
            }
            return Task.FromResult(total);
        }

        public async Task<bool> CanBeDeletedAsync(int purchaseId, int companyId)
            => await _purchaseMasterRepository.CanBeDeletedAsync(purchaseId, companyId);

        public async Task UpdatePurchaseOrderStatusAsync(int purchaseId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            await _purchaseMasterRepository.UpdateStatusAsync(purchaseId, status, workflowInstanceId, currentStageId);
        }
    }
}

