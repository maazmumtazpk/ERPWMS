using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services   // ✅ FIX: Folder ke saath match
{
    public class InventoryAdjustmentService(
        IStockAdjustmentRepository adjustmentRepository,
        IStockAdjustmentDetailRepository adjustmentDetailRepository,
        IStockLedgerRepository ledgerRepository,
        AppDbContext context) : IInventoryAdjustmentService
    {
        // ✅ Primary Constructor - fields ki zaroorat nahi

        public async Task<StockAdjustment> CreateAdjustmentAsync(
            StockAdjustment adjustment,
            List<StockAdjustmentDetail> details,
            string createdBy)   // ✅ FIX: parameter lowercase (body se match)
        {
            if (adjustment.AdjustmentType == "Decrease")
            {
                await ValidateStockAvailabilityAsync(
                    adjustment.ProductId,
                    adjustment.WarehouseId,
                    adjustment.BinId,
                    adjustment.Quantity,
                    adjustment.AdjustmentType);
            }

            var currentBalance = await ledgerRepository.GetCurrentBalanceAsync(
                adjustment.ProductId, adjustment.WarehouseId, adjustment.BinId);

            adjustment.PreviousQty = (int)currentBalance;
            adjustment.NewQty = adjustment.AdjustmentType == "Increase"
                ? adjustment.PreviousQty + adjustment.Quantity
                : adjustment.PreviousQty - adjustment.Quantity;

            adjustment.Status = "Draft";
            adjustment.CreatedBy = createdBy;   // ✅ already string
            adjustment.CreatedDate = DateTime.UtcNow;

            await adjustmentRepository.AddAsync(adjustment);

            foreach (var detail in details)
            {
                detail.AdjustmentId = adjustment.AdjustmentId;
                detail.PreviousQty = (int)currentBalance;
                detail.NewQty = adjustment.AdjustmentType == "Increase"
                    ? detail.PreviousQty + detail.Quantity
                    : detail.PreviousQty - detail.Quantity;
                detail.CreatedBy = createdBy;   // ✅ already string
                detail.CreatedDate = DateTime.UtcNow;
                await adjustmentDetailRepository.AddAsync(detail);
            }

            return adjustment;
        }

        public async Task<StockAdjustment> UpdateAdjustmentAsync(
            StockAdjustment adjustment,
            List<StockAdjustmentDetail> details,
            string updatedBy)
        {
            if (adjustment.Status != "Draft")
                throw new Exception("Only Draft adjustments can be updated.");

            var currentBalance = await ledgerRepository.GetCurrentBalanceAsync(
                adjustment.ProductId, adjustment.WarehouseId, adjustment.BinId);

            adjustment.PreviousQty = (int)currentBalance;
            adjustment.NewQty = adjustment.AdjustmentType == "Increase"
                ? adjustment.PreviousQty + adjustment.Quantity
                : adjustment.PreviousQty - adjustment.Quantity;

            adjustment.UpdatedBy = updatedBy;   // ✅ already string
            adjustment.UpdatedDate = DateTime.UtcNow;
            await adjustmentRepository.UpdateAsync(adjustment);

            var existingDetails = await adjustmentDetailRepository
                .GetByAdjustmentIdAsync(adjustment.AdjustmentId);

            foreach (var existing in existingDetails)
            {
                await adjustmentDetailRepository.DeleteAsync(existing.AdjustmentDetailId);
            }

            foreach (var detail in details)
            {
                detail.AdjustmentId = adjustment.AdjustmentId;
                detail.PreviousQty = (int)currentBalance;
                detail.NewQty = adjustment.AdjustmentType == "Increase"
                    ? detail.PreviousQty + detail.Quantity
                    : detail.PreviousQty - detail.Quantity;
                detail.CreatedBy = updatedBy;   // ✅ already string
                detail.CreatedDate = DateTime.UtcNow;
                await adjustmentDetailRepository.AddAsync(detail);
            }

            return adjustment;
        }

        public async Task<StockAdjustment?> GetAdjustmentByIdAsync(int adjustmentId)
        {
            return await adjustmentRepository.GetByIdAsync(adjustmentId);
        }

        public async Task<IEnumerable<StockAdjustment>> GetAllAdjustmentsAsync(
            int warehouseId, bool includeDeleted = false)
        {
            return await adjustmentRepository.GetByWarehouseIdAsync(warehouseId, includeDeleted);
        }

        public async Task<bool> SubmitAdjustmentAsync(int adjustmentId, int userId)
        {
            var adjustment = await adjustmentRepository.GetByIdAsync(adjustmentId);
            if (adjustment is null)   // ✅ simplified null check
                throw new Exception("Adjustment not found.");

            if (adjustment.Status != "Draft")
                throw new Exception("Only Draft adjustments can be submitted.");

            await adjustmentRepository.UpdateStatusAsync(adjustmentId, "Submitted");
            return true;
        }

        public async Task<bool> ApproveAdjustmentAsync(int adjustmentId, int userId)
        {
            if (!await CanBeApprovedAsync(adjustmentId))
                throw new Exception("Adjustment cannot be approved.");

            // ✅ FIX: userId.ToString() (repo string leta hai)
            await adjustmentRepository.ApproveAsync(
                adjustmentId, userId.ToString(), DateTime.UtcNow);
            return true;
        }

        public async Task<bool> RejectAdjustmentAsync(int adjustmentId, int userId)
        {
            var adjustment = await adjustmentRepository.GetByIdAsync(adjustmentId);
            if (adjustment is null)
                throw new Exception("Adjustment not found.");

            if (adjustment.Status != "Submitted")
                throw new Exception("Only Submitted adjustments can be rejected.");

            // ✅ FIX: userId.ToString()
            await adjustmentRepository.RejectAsync(adjustmentId, userId.ToString());
            return true;
        }

        public async Task<bool> PostAdjustmentAsync(int adjustmentId, int userId)
        {
            if (!await CanBePostedAsync(adjustmentId))
                throw new Exception("Adjustment cannot be posted.");

            var adjustment = await adjustmentRepository.GetByIdAsync(adjustmentId);
            if (adjustment is null)
                throw new Exception("Adjustment not found.");

            // ✅ FIX: userId.ToString()
            await adjustmentRepository.PostAsync(adjustmentId, userId.ToString());

            await CreateLedgerEntryForAdjustmentAsync(adjustment, userId);

            return true;
        }

        public async Task<bool> DeleteAdjustmentAsync(int adjustmentId, string deletedBy)
        {
            var adjustment = await adjustmentRepository.GetByIdAsync(adjustmentId);
            if (adjustment is null)
                throw new Exception("Adjustment not found.");

            if (adjustment.Status != "Draft")
                throw new Exception("Only Draft adjustments can be deleted.");

            await adjustmentRepository.UpdateStatusAsync(adjustmentId, "Deleted");
            return true;
        }

        public async Task<bool> CanBePostedAsync(int adjustmentId)
        {
            return await adjustmentRepository.CanBePostedAsync(adjustmentId);
        }

        public async Task<bool> CanBeApprovedAsync(int adjustmentId)
        {
            return await adjustmentRepository.CanBeApprovedAsync(adjustmentId);
        }

        public async Task UpdateAdjustmentStatusAsync(int adjustmentId, string status)
        {
            await adjustmentRepository.UpdateStatusAsync(adjustmentId, status);
        }

        public async Task ValidateStockAvailabilityAsync(
            int productId, int warehouseId, int binId,
            int quantity, string adjustmentType)
        {
            if (adjustmentType == "Decrease")
            {
                var currentBalance = await ledgerRepository.GetCurrentBalanceAsync(
                    productId, warehouseId, binId);

                if (currentBalance < quantity)
                    throw new Exception(
                        $"Insufficient stock. Available: {currentBalance}, Requested: {quantity}");
            }
        }

        public async Task<StockLedger> CreateLedgerEntryForAdjustmentAsync(
            StockAdjustment adjustment, int userId)
        {
            var ledger = new StockLedger
            {
                ProductId = adjustment.ProductId,
                WarehouseId = adjustment.WarehouseId,
                BinId = adjustment.BinId,
                TransactionType = adjustment.AdjustmentType == "Increase" ? "ADJ_IN" : "ADJ_OUT",
                ReferenceDocNo = adjustment.AdjustmentId.ToString(),
                ReferenceDocType = "StockAdjustment",
                Quantity = adjustment.Quantity,
                Balance = adjustment.NewQty,
                UnitCost = 0,
                TransactionDate = DateTime.UtcNow,
                CreatedBy = userId.ToString(),   // ✅ int -> string
                CreatedDate = DateTime.UtcNow
            };

            return await ledgerRepository.AddAsync(ledger);
        }
    }
}