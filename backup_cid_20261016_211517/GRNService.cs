using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;

        public GrnService(
            IGrnRepository grnRepository,
            IGrndetailRepository grnDetailRepository,
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository,
            AppDbContext context)
        {
            _grnRepository = grnRepository;
            _grnDetailRepository = grnDetailRepository;
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
            _context = context;
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
                throw new InvalidOperationException("Grn Number already exists.");

            grn.Status = "Draft";
            grn.CreatedBy = createdBy;
            grn.CreatedDate = DateTime.UtcNow;

            await _grnRepository.AddAsync(grn);

            foreach (var detail in details)
            {
                detail.GrnId = grn.GrnId;
                detail.CreatedBy = createdBy;
                detail.CreatedDate = DateTime.UtcNow;
                await _grnDetailRepository.AddAsync(detail);
            }

            return grn;
        }

        public async Task UpdateGrnAsync(
            Grn grn,
            List<Grndetail> details,
            string updatedBy)
        {
            grn.UpdatedBy = updatedBy;
            grn.UpdatedDate = DateTime.UtcNow;

            await _grnRepository.UpdateAsync(grn);

            var existingDetails = await _grnDetailRepository.GetByGrnIdAsync(grn.GrnId);

            foreach (var existing in existingDetails)
                await _grnDetailRepository.DeleteAsync(existing.GrndetailId);

            foreach (var detail in details)
            {
                detail.GrnId = grn.GrnId;
                detail.CreatedBy = updatedBy;
                detail.CreatedDate = DateTime.UtcNow;
                await _grnDetailRepository.AddAsync(detail);
            }
        }

        public async Task<bool> DeleteGrnAsync(
            int grnId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(grnId, companyId))
                throw new InvalidOperationException("Cannot delete Grn as it may have associated returns.");

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
            await Task.CompletedTask;
        }

        // =========================================================
        // INVENTORY UPDATE ON POST
        // =========================================================
        public async Task UpdateInventoryOnPostAsync(int grnId)
        {
            var grn = await _grnRepository.GetByIdAsync(grnId, 0);
            if (grn == null) return;

            var details = await _context.Grndetails
                .Where(d => d.GrnId == grnId && (d.IsDeleted == null || d.IsDeleted == false))
                .ToListAsync();

            if (!details.Any()) return;

            var now = DateTime.UtcNow;

            foreach (var detail in details)
            {
                if (detail.AcceptedQty <= 0) continue;

                // Product comes IN from supplier
                _context.StockLedgers.Add(new StockLedger
                {
                    ProductId = detail.ProductId,
                    WarehouseId = grn.WarehouseId,
                    Quantity = detail.AcceptedQty,
                    TransactionType = "IN",
                    ReferenceDocType = "GRN",
                    ReferenceDocNo = grn.GrnNo ?? grnId.ToString(),
                    ReferenceId = grnId,
                    CreatedBy = "System",
                    CreatedDate = now,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
