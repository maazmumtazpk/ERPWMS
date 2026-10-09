using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class GrnService : IGrnService
    {
        private readonly IGrnRepository _grnRepository;
        private readonly IGrndetailRepository _grnDetailRepository;
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public GrnService(
            IGrnRepository grnRepository,
            IGrndetailRepository grnDetailRepository,
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository)
        {
            _grnRepository = grnRepository;
            _grnDetailRepository = grnDetailRepository;
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
        }

        public async Task<IEnumerable<Grn>> GetAllAsync(int companyId, bool includeDeleted = false)
            => await _grnRepository.GetAllAsync(companyId, includeDeleted);

        public async Task<Grn?> GetByIdAsync(int grnId, int companyId)
            => await _grnRepository.GetByIdAsync(grnId, companyId);

        public async Task<Grn> CreateGrnAsync(
            Grn grn,
            List<Grndetail> details,
            string createdBy)
        {
            if (!await _grnRepository.IsGrnNoUniqueAsync(grn.GrnNo, grn.Warehouse?.Branch?.CompanyId ?? 0, null))
                throw new Exception("Grn Number already exists.");

            grn.Status = "Draft";
            grn.CreatedBy = createdBy.ToString();
            grn.CreatedDate = DateTime.UtcNow;

            await _grnRepository.AddAsync(grn);

            foreach (var detail in details)
            {
                detail.GrnId = grn.GrnId;
                detail.CreatedBy = createdBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _grnDetailRepository.AddAsync(detail);
            }

            return grn;
        }

        // ? FIXED — MISSING METHOD
        public async Task UpdateGrnAsync(
            Grn grn,
            List<Grndetail> details,
            string updatedBy)
        {
            grn.UpdatedBy = updatedBy.ToString();
            grn.UpdatedDate = DateTime.UtcNow;

            await _grnRepository.UpdateAsync(grn);

            var existingDetails = await _grnDetailRepository.GetByGrnIdAsync(grn.GrnId);

            foreach (var existing in existingDetails)
            {
                await _grnDetailRepository.DeleteAsync(existing.GrndetailId);
            }

            foreach (var detail in details)
            {
                detail.GrnId = grn.GrnId;
                detail.CreatedBy = updatedBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _grnDetailRepository.AddAsync(detail);
            }
        }

        // ? FIXED — MISSING METHOD
        public async Task<bool> DeleteGrnAsync(
            int grnId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(grnId, companyId))
                throw new Exception("Cannot delete Grn as it may have associated returns.");

            await _grnRepository.SoftDeleteAsync(grnId, companyId, deletedBy);
            return true;
        }

        public async Task<bool> PostGrnAsync(int grnId, int companyId, int userId)
        {
            var grn = await _grnRepository.GetByIdAsync(grnId, companyId);
            if (grn == null) return false;

            grn.Status = "Posted";
            grn.UpdatedBy = userId.ToString();
            grn.UpdatedDate = DateTime.UtcNow;

            await _grnRepository.UpdateAsync(grn);
            await UpdateInventoryOnPostAsync(grnId);
            return true;
        }

        public async Task<bool> RejectGrnAsync(int grnId, int companyId, int userId)
        {
            var grn = await _grnRepository.GetByIdAsync(grnId, companyId);
            if (grn == null) return false;

            grn.Status = "Rejected";
            grn.UpdatedBy = userId.ToString();
            grn.UpdatedDate = DateTime.UtcNow;

            await _grnRepository.UpdateAsync(grn);
            return true;
        }

        public async Task<Grn?> GetByGrnNoAsync(string grnNo, int companyId)
            => await _grnRepository.GetByGrnNoAsync(grnNo, companyId);

        public async Task<IEnumerable<Grn>> GetByPurchaseIdAsync(int purchaseId)
            => await _grnRepository.GetByPurchaseIdAsync(purchaseId);

        public async Task<IEnumerable<Grn>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
            => await _grnRepository.GetByDateRangeAsync(fromDate, toDate, companyId);

        public async Task<IEnumerable<Grn>> GetByStatusAsync(string status, int companyId)
            => await _grnRepository.GetByStatusAsync(status, companyId);

        public async Task<bool> IsGrnNoUniqueAsync(string grnNo, int companyId, int? excludeId = null)
            => await _grnRepository.IsGrnNoUniqueAsync(grnNo, companyId, excludeId);

        public async Task<bool> CanBeDeletedAsync(int grnId, int companyId)
            => await _grnRepository.CanBeDeletedAsync(grnId, companyId);

        public async Task UpdateGrnStatusAsync(int grnId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            await _grnRepository.UpdateStatusAsync(grnId, status, workflowInstanceId, currentStageId);
        }

        public async Task ValidateGrnAgainstPurchaseOrderAsync(int purchaseId, List<Grndetail> details)
        {
            var purchaseDetails = await _purchaseDetailRepository.GetByPurchaseIdAsync(purchaseId);
            // Validation logic — extend as needed
            await Task.CompletedTask;
        }

        public async Task UpdateInventoryOnPostAsync(int grnId)
        {
            // Inventory ledger update logic
            await Task.CompletedTask;
        }
    }
}