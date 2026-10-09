using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class PurchaseReturnService : IPurchaseReturnService
    {
        private readonly IPurchaseReturnRepository _purchaseReturnRepository;
        private readonly IPurchaseReturnDetailRepository _purchaseReturnDetailRepository;
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;

        public PurchaseReturnService(
            IPurchaseReturnRepository purchaseReturnRepository,
            IPurchaseReturnDetailRepository purchaseReturnDetailRepository,
            IPurchaseMasterRepository purchaseMasterRepository)
        {
            _purchaseReturnRepository = purchaseReturnRepository;
            _purchaseReturnDetailRepository = purchaseReturnDetailRepository;
            _purchaseMasterRepository = purchaseMasterRepository;
        }

        public async Task<IEnumerable<PurchaseReturn>> GetAllAsync(int companyId, bool includeDeleted = false)
            => await _purchaseReturnRepository.GetAllAsync(companyId, includeDeleted);

        public async Task<PurchaseReturn?> GetByIdAsync(int returnId, int companyId)
            => await _purchaseReturnRepository.GetByIdAsync(returnId, companyId);

        public async Task<PurchaseReturn> CreateReturnAsync(
            PurchaseReturn purchaseReturn,
            List<PurchaseReturnDetail> details,
            string createdBy)
        {
            if (!await _purchaseReturnRepository.IsReturnNoUniqueAsync(purchaseReturn.ReturnNo, 0, null))
                throw new Exception("Return Number already exists.");

            purchaseReturn.Status = "Draft";
            purchaseReturn.CreatedBy = createdBy.ToString();
            purchaseReturn.CreatedDate = DateTime.UtcNow;

            await _purchaseReturnRepository.AddAsync(purchaseReturn);

            foreach (var detail in details)
            {
                detail.ReturnId = purchaseReturn.ReturnId;
                detail.CreatedBy = createdBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _purchaseReturnDetailRepository.AddAsync(detail);
            }

            return purchaseReturn;
        }

        // ? FIXED — MISSING METHOD
        public async Task UpdateReturnAsync(
            PurchaseReturn purchaseReturn,
            List<PurchaseReturnDetail> details,
            string updatedBy)
        {
            purchaseReturn.UpdatedBy = updatedBy.ToString();
            purchaseReturn.UpdatedDate = DateTime.UtcNow;

            await _purchaseReturnRepository.UpdateAsync(purchaseReturn);

            var existingDetails = await _purchaseReturnDetailRepository.GetByReturnIdAsync(purchaseReturn.ReturnId);

            foreach (var existing in existingDetails)
            {
                await _purchaseReturnDetailRepository.DeleteAsync(existing.ReturnDetailId);
            }

            foreach (var detail in details)
            {
                detail.ReturnId = purchaseReturn.ReturnId;
                detail.CreatedBy = updatedBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _purchaseReturnDetailRepository.AddAsync(detail);
            }
        }

        // ? FIXED — MISSING METHOD
        public async Task<bool> DeleteReturnAsync(
            int returnId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(returnId, companyId))
                throw new Exception("Cannot delete Purchase Return as it may be in use.");

            await _purchaseReturnRepository.SoftDeleteAsync(returnId, companyId, deletedBy);
            return true;
        }

        public async Task<bool> SubmitReturnAsync(int returnId, int companyId, int userId)
        {
            var purchaseReturn = await _purchaseReturnRepository.GetByIdAsync(returnId, companyId);
            if (purchaseReturn == null) return false;

            purchaseReturn.Status = "Submitted";
            purchaseReturn.UpdatedBy = userId.ToString();
            purchaseReturn.UpdatedDate = DateTime.UtcNow;

            await _purchaseReturnRepository.UpdateAsync(purchaseReturn);
            return true;
        }

        public async Task<bool> ApproveReturnAsync(int returnId, int companyId, int userId)
        {
            var purchaseReturn = await _purchaseReturnRepository.GetByIdAsync(returnId, companyId);
            if (purchaseReturn == null) return false;

            await _purchaseReturnRepository.ApproveAsync(returnId, userId.ToString(), DateTime.UtcNow);
            await UpdateInventoryOnApproveAsync(returnId);
            return true;
        }

        public async Task<bool> RejectReturnAsync(int returnId, int companyId, int userId)
        {
            var purchaseReturn = await _purchaseReturnRepository.GetByIdAsync(returnId, companyId);
            if (purchaseReturn == null) return false;

            purchaseReturn.Status = "Rejected";
            purchaseReturn.UpdatedBy = userId.ToString();
            purchaseReturn.UpdatedDate = DateTime.UtcNow;

            await _purchaseReturnRepository.UpdateAsync(purchaseReturn);
            return true;
        }

        public async Task<PurchaseReturn?> GetByReturnNoAsync(string returnNo, int companyId)
            => await _purchaseReturnRepository.GetByReturnNoAsync(returnNo, companyId);

        public async Task<IEnumerable<PurchaseReturn>> GetByPurchaseIdAsync(int purchaseId)
            => await _purchaseReturnRepository.GetByPurchaseIdAsync(purchaseId);

        public async Task<IEnumerable<PurchaseReturn>> GetByGrnIdAsync(int grnId)
            => await _purchaseReturnRepository.GetByGrnIdAsync(grnId);

        public async Task<IEnumerable<PurchaseReturn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
            => await _purchaseReturnRepository.GetByDateRangeAsync(fromDate, toDate, companyId);

        public async Task<IEnumerable<PurchaseReturn>> GetByStatusAsync(string status, int companyId)
            => await _purchaseReturnRepository.GetByStatusAsync(status, companyId);

        public async Task<bool> IsReturnNoUniqueAsync(string returnNo, int companyId, int? excludeId = null)
            => await _purchaseReturnRepository.IsReturnNoUniqueAsync(returnNo, companyId, excludeId);

        public async Task<bool> CanBeDeletedAsync(int returnId, int companyId)
            => await _purchaseReturnRepository.CanBeDeletedAsync(returnId, companyId);

        public async Task UpdateReturnStatusAsync(int returnId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            await _purchaseReturnRepository.UpdateStatusAsync(returnId, status, workflowInstanceId, currentStageId);
        }

        public async Task ValidateReturnAgainstPurchaseAsync(int purchaseId, List<PurchaseReturnDetail> details)
        {
            var purchase = await _purchaseMasterRepository.GetByIdAsync(purchaseId, 0);
            if (purchase == null)
                throw new Exception("Purchase Order not found.");

            await Task.CompletedTask;
        }

        public async Task UpdateInventoryOnApproveAsync(int returnId)
        {
            // Inventory ledger logic
            await Task.CompletedTask;
        }
    }
}