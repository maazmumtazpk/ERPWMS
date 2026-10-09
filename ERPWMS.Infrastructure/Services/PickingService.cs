using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class PickingService : IPickingService
    {
        private readonly IPickingListRepository _pickingListRepository;
        private readonly IPickingDetailRepository _pickingDetailRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;
        private readonly IDispatchMasterRepository _dispatchMasterRepository;
        private readonly AppDbContext _context;

        public PickingService(
            IPickingListRepository pickingListRepository,
            IPickingDetailRepository pickingDetailRepository,
            ISalesDetailRepository salesDetailRepository,
            IDispatchMasterRepository dispatchMasterRepository,
            AppDbContext context)
        {
            _pickingListRepository = pickingListRepository;
            _pickingDetailRepository = pickingDetailRepository;
            _salesDetailRepository = salesDetailRepository;
            _dispatchMasterRepository = dispatchMasterRepository;
            _context = context;
        }

        public async Task<IEnumerable<PickingList>> GetAllAsync(
            int companyId,
            bool includeDeleted = false)
        {
            return await _pickingListRepository
                .GetAllAsync(companyId, includeDeleted);
        }

        public async Task<PickingList?> GetByIdAsync(
            int pickingId,
            int companyId)
        {
            return await _pickingListRepository
                .GetByIdAsync(pickingId, companyId);
        }

        public async Task<PickingList?> GetByPickingNoAsync(
            string pickingNo,
            int companyId)
        {
            return await _pickingListRepository
                .GetByPickingNoAsync(pickingNo, companyId);
        }

        public async Task<IEnumerable<PickingList>> GetBySalesIdAsync(
            int salesId)
        {
            return await _pickingListRepository
                .GetBySalesIdAsync(salesId);
        }

        public async Task<IEnumerable<PickingList>> GetByStatusAsync(
            string status,
            int companyId)
        {
            return await _pickingListRepository
                .GetByStatusAsync(status, companyId);
        }

        public async Task<IEnumerable<PickingList>> GetByAssignedToAsync(
            int userId,
            int companyId)
        {
            return await _pickingListRepository
                .GetByAssignedToAsync(userId, companyId);
        }

        // ? FIXED: parameter name CreatedBy ? createdBy (consistent)
        public async Task<PickingList> CreatePickingListAsync(
            PickingList picking,
            List<PickingDetail> details,
            string createdBy)
        {
            if (!await _pickingListRepository.IsPickingNoUniqueAsync(
                    picking.PickingNo,
                    picking.Warehouse.Branch.CompanyId,
                    null))
            {
                throw new Exception("Picking Number already exists.");
            }

            picking.Status = "Draft";
            picking.CreatedBy = createdBy.ToString();
            picking.CreatedDate = DateTime.UtcNow;

            await _pickingListRepository.AddAsync(picking);

            foreach (var detail in details)
            {
                detail.PickingId = picking.PickingId;
                detail.CreatedBy = createdBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;

                await _pickingDetailRepository.AddAsync(detail);
            }

            return picking;
        }

        // ? FIXED: int updatedBy ? string updatedBy
        public async Task UpdatePickingListAsync(
            PickingList picking,
            List<PickingDetail> details,
            string updatedBy)
        {
            picking.UpdatedBy = updatedBy.ToString();
            picking.UpdatedDate = DateTime.UtcNow;

            await _pickingListRepository.UpdateAsync(picking);

            var existingDetails =
                await _pickingDetailRepository
                    .GetByPickingIdAsync(picking.PickingId);

            foreach (var existing in existingDetails)
            {
                await _pickingDetailRepository
                    .DeleteAsync(existing.PickingDetailId);
            }

            foreach (var detail in details)
            {
                detail.PickingId = picking.PickingId;
                detail.CreatedBy = updatedBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;

                await _pickingDetailRepository.AddAsync(detail);
            }
        }

        // ? FIXED: int deletedBy ? string deletedBy
        public async Task<bool> DeletePickingListAsync(
            int pickingId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(pickingId, companyId))
            {
                throw new Exception(
                    "Cannot delete Picking List as it may have been used.");
            }

            await _pickingListRepository
                .SoftDeleteAsync(
                    pickingId,
                    companyId,
                    deletedBy);

            return true;
        }

        // ? FIXED: int assignedBy ? string assignedBy
        public async Task<bool> AssignPickerAsync(
            int pickingId,
            int assignedTo,
            string assignedBy)
        {
            var picking =
                await _pickingListRepository.GetByIdAsync(
                    pickingId,
                    0);

            if (picking == null)
                return false;

            picking.AssignedTo = assignedTo;
            picking.UpdatedBy = assignedBy;
            picking.UpdatedDate = DateTime.UtcNow;

            await _pickingListRepository.UpdateAsync(picking);

            return true;
        }

        // ? FIXED: int pickedBy ? string pickedBy
        public async Task<bool> ConfirmPickingAsync(
            int pickingId,
            string pickedBy)
        {
            var picking =
                await _pickingListRepository.GetByIdAsync(
                    pickingId,
                    0);

            if (picking == null)
                return false;

            if (picking.Status != "Draft" &&
                picking.Status != "Assigned")
            {
                throw new Exception(
                    "Picking list cannot be confirmed.");
            }

            picking.Status = "Picked";
            picking.PickedBy = pickedBy;
            picking.UpdatedBy = pickedBy;
            picking.UpdatedDate = DateTime.UtcNow;

            await _pickingListRepository.UpdateAsync(picking);

            var details =
                await _pickingDetailRepository
                    .GetByPickingIdAsync(pickingId);

            foreach (var detail in details)
            {
                var salesDetail =
                    await _salesDetailRepository
                        .GetByIdAsync(detail.SalesDetailId);

                if (salesDetail != null)
                {
                    salesDetail.PickedQty =
                        (salesDetail.PickedQty ?? 0)
                        + detail.PickedQty;

                    await _salesDetailRepository
                        .UpdateAsync(salesDetail);
                }
            }

            return true;
        }

        // ? FIXED: int verifiedBy ? string verifiedBy
        public async Task<bool> VerifyPickingAsync(
            int pickingId,
            string verifiedBy)
        {
            var picking =
                await _pickingListRepository.GetByIdAsync(
                    pickingId,
                    0);

            if (picking == null)
                return false;

            if (picking.Status != "Picked")
            {
                throw new Exception(
                    "Only picked lists can be verified.");
            }

            picking.Status = "Verified";
            picking.VerifiedBy = verifiedBy;
            picking.UpdatedBy = verifiedBy;
            picking.UpdatedDate = DateTime.UtcNow;

            await _pickingListRepository.UpdateAsync(picking);

            return true;
        }

        public async Task<bool> IsPickingNoUniqueAsync(
            string pickingNo,
            int companyId,
            int? excludeId = null)
        {
            return await _pickingListRepository
                .IsPickingNoUniqueAsync(
                    pickingNo,
                    companyId,
                    excludeId);
        }

        public async Task<bool> CanBeDeletedAsync(
            int pickingId,
            int companyId)
        {
            return await _pickingListRepository
                .CanBeDeletedAsync(
                    pickingId,
                    companyId);
        }

        public async Task UpdatePickingStatusAsync(
            int pickingId,
            string status,
            int? workflowInstanceId = null,
            int? currentStageId = null)
        {
            await _pickingListRepository
                .UpdateStatusAsync(
                    pickingId,
                    status,
                    workflowInstanceId,
                    currentStageId);
        }

        // ? FIXED: parameter name CreatedBy ? createdBy
        public async Task<DispatchMaster> GenerateDispatchFromPickingAsync(
            int pickingId,
            string createdBy)
        {
            var picking =
                await _pickingListRepository.GetByIdAsync(
                    pickingId,
                    0);

            if (picking == null)
                throw new Exception("Picking list not found.");

            if (picking.Status != "Verified")
            {
                throw new Exception(
                    "Only verified picking lists can be dispatched.");
            }

            var dispatchNo =
                $"DISP-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            var dispatch = new DispatchMaster
            {
                SalesId = picking.SalesId,
                WarehouseId = picking.WarehouseId,
                DispatchNo = dispatchNo,
                DispatchDate = DateTime.UtcNow,
                CarrierName = "TBD",
                Status = "Draft",
                CreatedBy = createdBy.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await _dispatchMasterRepository
                .AddAsync(dispatch);

            var pickingDetails =
                await _pickingDetailRepository
                    .GetByPickingIdAsync(pickingId);

            var dispatchDetails =
                new List<DispatchDetail>();

            foreach (var pd in pickingDetails)
            {
                var dd = new DispatchDetail
                {
                    DispatchId = dispatch.DispatchId,
                    SalesDetailId = pd.SalesDetailId,
                    ProductId = pd.ProductId,
                    Quantity = pd.PickedQty,
                    BatchNo = pd.BatchNo,
                    SerialNumbers = pd.SerialNumbers,
                    CreatedBy = createdBy.ToString(),
                    CreatedDate = DateTime.UtcNow
                };

                dispatchDetails.Add(dd);
            }

            if (dispatchDetails.Count > 0)
            {
                await _context.DispatchDetails
                    .AddRangeAsync(dispatchDetails);

                await _context.SaveChangesAsync();
            }

            return dispatch;
        }
    }
}