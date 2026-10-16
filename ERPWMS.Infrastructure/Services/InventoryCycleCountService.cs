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
    public class InventoryCycleCountService(
        ICycleCountRepository cycleCountRepository,
        ICycleCountDetailRepository cycleCountDetailRepository,
        IInventoryAdjustmentService adjustmentService) : IInventoryCycleCountService
    {
        // âš ï¸ ledgerRepository aur context remove kiye (use nahi ho rahe the)

        public async Task<CycleCount> CreateCycleCountAsync(
            CycleCount cycleCount,
            List<CycleCountDetail> details,
            string createdBy)
        {
            foreach (var detail in details)
            {
                detail.Variance = detail.ActualQty - detail.SystemQty;
            }

            cycleCount.Variance = details.Sum(d => d.Variance);
            cycleCount.SystemQty = details.Sum(d => d.SystemQty);
            cycleCount.ActualQty = details.Sum(d => d.ActualQty);
            cycleCount.Status = "Draft";
            cycleCount.CountedDate = DateTime.UtcNow;
            cycleCount.CountedBy = createdBy;
            cycleCount.CreatedBy = createdBy;
            cycleCount.CreatedDate = DateTime.UtcNow;

            await cycleCountRepository.AddAsync(cycleCount);

            foreach (var detail in details)
            {
                detail.CountId = cycleCount.CycleCountId;
                detail.CreatedBy = createdBy;
                detail.CreatedDate = DateTime.UtcNow;
                await cycleCountDetailRepository.AddAsync(detail);
            }

            return cycleCount;
        }

        public async Task<CycleCount> UpdateCycleCountAsync(
            CycleCount cycleCount,
            List<CycleCountDetail> details,
            string updatedBy)
        {
            if (cycleCount.Status != "Draft")
                throw new InvalidOperationException("Only Draft cycle counts can be updated.");

            foreach (var detail in details)
            {
                detail.Variance = detail.ActualQty - detail.SystemQty;
            }

            cycleCount.Variance = details.Sum(d => d.Variance);
            cycleCount.SystemQty = details.Sum(d => d.SystemQty);
            cycleCount.ActualQty = details.Sum(d => d.ActualQty);
            cycleCount.UpdatedBy = updatedBy;
            cycleCount.UpdatedDate = DateTime.UtcNow;
            await cycleCountRepository.UpdateAsync(cycleCount);

            var existingDetails = await cycleCountDetailRepository
                .GetByCycleCountIdAsync(cycleCount.CycleCountId);

            foreach (var existing in existingDetails)
            {
                await cycleCountDetailRepository.DeleteAsync(existing.CountDetailId);
            }

            foreach (var detail in details)
            {
                detail.CountId = cycleCount.CycleCountId;
                detail.CreatedBy = updatedBy;
                detail.CreatedDate = DateTime.UtcNow;
                await cycleCountDetailRepository.AddAsync(detail);
            }

            return cycleCount;
        }

        public async Task<CycleCount?> GetCycleCountByIdAsync(int cycleCountId)
        {
            return await cycleCountRepository.GetByIdAsync(cycleCountId);
        }

        public async Task<IEnumerable<CycleCount>> GetAllCycleCountsAsync(
            int warehouseId, bool includeDeleted = false)
        {
            return await cycleCountRepository.GetByWarehouseIdAsync(warehouseId, includeDeleted);
        }

        public async Task<bool> SubmitCycleCountAsync(int cycleCountId, int userId)
        {
            var cycleCount = await cycleCountRepository.GetByIdAsync(cycleCountId);
            if (cycleCount is null)
                throw new InvalidOperationException("Cycle count not found.");

            if (cycleCount.Status != "Draft")
                throw new InvalidOperationException("Only Draft cycle counts can be submitted.");

            await cycleCountRepository.UpdateStatusAsync(cycleCountId, "Submitted");
            return true;
        }

        public async Task<bool> VerifyCycleCountAsync(int cycleCountId, int userId)
        {
            if (!await CanBeVerifiedAsync(cycleCountId))
                throw new InvalidOperationException("Cycle count cannot be verified.");

            // âœ… FIX: userId.ToString() â€” VerifyAsync string leta hai
            await cycleCountRepository.VerifyAsync(cycleCountId, userId.ToString(), DateTime.UtcNow);
            return true;
        }

        public async Task<bool> RejectCycleCountAsync(int cycleCountId, int userId)
        {
            var cycleCount = await cycleCountRepository.GetByIdAsync(cycleCountId);
            if (cycleCount is null)
                throw new InvalidOperationException("Cycle count not found.");

            if (cycleCount.Status != "Submitted")
                throw new InvalidOperationException("Only Submitted cycle counts can be rejected.");

            // âœ… FIX: userId.ToString() â€” RejectAsync string leta hai
            await cycleCountRepository.RejectAsync(cycleCountId, userId.ToString());
            return true;
        }

        public async Task<bool> PostCycleCountAsync(int cycleCountId, int userId)
        {
            if (!await CanBePostedAsync(cycleCountId))
                throw new InvalidOperationException("Cycle count cannot be posted.");

            var cycleCount = await cycleCountRepository.GetByIdAsync(cycleCountId);
            if (cycleCount is null)
                throw new InvalidOperationException("Cycle count not found.");

            if (cycleCount.Variance != 0)
            {
                await CreateAdjustmentFromVarianceAsync(cycleCount, userId);
            }

            // âœ… FIX: userId.ToString() â€” PostAsync string leta hai
            await cycleCountRepository.PostAsync(cycleCountId, userId.ToString());
            return true;
        }

        public async Task<bool> DeleteCycleCountAsync(int cycleCountId, string deletedBy)
        {
            var cycleCount = await cycleCountRepository.GetByIdAsync(cycleCountId);
            if (cycleCount is null)
                throw new InvalidOperationException("Cycle count not found.");

            if (cycleCount.Status != "Draft")
                throw new InvalidOperationException("Only Draft cycle counts can be deleted.");

            await cycleCountRepository.UpdateStatusAsync(cycleCountId, "Deleted");
            return true;
        }

        public async Task<bool> CanBeVerifiedAsync(int cycleCountId)
        {
            return await cycleCountRepository.CanBeVerifiedAsync(cycleCountId);
        }

        public async Task<bool> CanBePostedAsync(int cycleCountId)
        {
            return await cycleCountRepository.CanBePostedAsync(cycleCountId);
        }

        public async Task UpdateCycleCountStatusAsync(int cycleCountId, string status)
        {
            await cycleCountRepository.UpdateStatusAsync(cycleCountId, status);
        }

        public async Task<int> CalculateVarianceAsync(int cycleCountId)
        {
            return await cycleCountDetailRepository.GetTotalVarianceAsync(cycleCountId);
        }

        // âœ… FIX: Return type Task<StockAdjustment> (null! use kiya jab null return karna ho)
        public async Task<StockAdjustment> CreateAdjustmentFromVarianceAsync(
            CycleCount cycleCount, int userId)
        {
            var details = await cycleCountDetailRepository
                .GetByCycleCountIdAsync(cycleCount.CycleCountId);

            var adjustmentDetails = new List<StockAdjustmentDetail>();

            foreach (var detail in details)
            {
                if (detail.Variance != 0)
                {
                    var adjDetail = new StockAdjustmentDetail
                    {
                        ProductId = detail.ProductId,
                        BinId = detail.BinId,
                        Quantity = Math.Abs(detail.Variance),
                        PreviousQty = detail.SystemQty,
                        NewQty = detail.ActualQty
                    };
                    adjustmentDetails.Add(adjDetail);
                }
            }

            if (adjustmentDetails.Count == 0)
                return null!;   // âœ… Interface ke saath match

            var adjustment = new StockAdjustment
            {
                WarehouseId = cycleCount.WarehouseId,
                ProductId = cycleCount.ProductId,
                BinId = cycleCount.BinId,
                AdjustmentType = "CycleCount",
                Quantity = adjustmentDetails.Sum(d => d.Quantity),
                Reason = $"Cycle Count Adjustment - Count #{cycleCount.CycleCountId}",
                Status = "Approved",
                ApprovedBy = userId.ToString(),   // âœ… FIX: int -> string
                ApprovedDate = DateTime.UtcNow
            };

            // âœ… FIX: userId int pass kiya (string nahi)
            await adjustmentService.CreateAdjustmentAsync(
                adjustment, adjustmentDetails, userId.ToString());

            await adjustmentService.PostAdjustmentAsync(adjustment.AdjustmentId, userId);

            return adjustment;
        }
    }
}
