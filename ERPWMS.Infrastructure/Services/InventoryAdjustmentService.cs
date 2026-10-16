using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class InventoryAdjustmentService(
        IStockAdjustmentRepository adjustmentRepository,
        IStockAdjustmentDetailRepository adjustmentDetailRepository,
        IStockLedgerRepository ledgerRepository,
        AppDbContext context) : IInventoryAdjustmentService
    {
        public async Task<StockAdjustment> CreateAdjustmentAsync(
            StockAdjustment adjustment,
            List<StockAdjustmentDetail> details,
            string createdBy)
        {
            if (adjustment is null)
                throw new ArgumentNullException(nameof(adjustment));

            if (details is null || !details.Any())
                throw new ArgumentException("Adjustment must have at least one detail.", nameof(details));

            if (adjustment.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

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
            adjustment.CreatedBy = createdBy;
            adjustment.CreatedDate = DateTime.UtcNow;

            await adjustmentRepository.AddAsync(adjustment);

            foreach (var detail in details)
            {
                if (detail.Quantity <= 0)
                    throw new InvalidOperationException($"Invalid quantity for product {detail.ProductId}.");

                detail.AdjustmentId = adjustment.AdjustmentId;
                detail.PreviousQty = (int)currentBalance;
                detail.NewQty = adjustment.AdjustmentType == "Increase"
                    ? detail.PreviousQty + detail.Quantity
                    : detail.PreviousQty - detail.Quantity;
                detail.CreatedBy = createdBy;
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
            if (adjustment is null)
                throw new ArgumentNullException(nameof(adjustment));

            if (adjustment.Status != "Draft")
                throw new InvalidOperationException("Only Draft adjustments can be updated.");

            if (adjustment.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            // FIXED: Validate stock on update too
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

            adjustment.UpdatedBy = updatedBy;
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
                if (detail.Quantity <= 0)
                    throw new InvalidOperationException($"Invalid quantity for product {detail.ProductId}.");

                detail.AdjustmentId = adjustment.AdjustmentId;
                detail.PreviousQty = (int)currentBalance;
                detail.NewQty = adjustment.AdjustmentType == "Increase"
                    ? detail.PreviousQty + detail.Quantity
                    : detail.PreviousQty - detail.Quantity;
                detail.CreatedBy = updatedBy;
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
            if (adjustment is null)
                throw new InvalidOperationException("Adjustment not found.");

            if (adjustment.Status != "Draft")
                throw new InvalidOperationException("Only Draft adjustments can be submitted.");

            await adjustmentRepository.UpdateStatusAsync(adjustmentId, "Submitted");
            return true;
        }

        public async Task<bool> ApproveAdjustmentAsync(int adjustmentId, int userId)
        {
            if (!await CanBeApprovedAsync(adjustmentId))
                throw new InvalidOperationException("Adjustment cannot be approved.");

            await adjustmentRepository.ApproveAsync(
                adjustmentId, userId.ToString(), DateTime.UtcNow);
            return true;
        }

        public async Task<bool> RejectAdjustmentAsync(int adjustmentId, int userId)
        {
            var adjustment = await adjustmentRepository.GetByIdAsync(adjustmentId);
            if (adjustment is null)
                throw new InvalidOperationException("Adjustment not found.");

            if (adjustment.Status != "Submitted")
                throw new InvalidOperationException("Only Submitted adjustments can be rejected.");

            await adjustmentRepository.RejectAsync(adjustmentId, userId.ToString());
            return true;
        }

        public async Task<bool> PostAdjustmentAsync(int adjustmentId, int userId)
        {
            if (!await CanBePostedAsync(adjustmentId))
                throw new InvalidOperationException("Adjustment cannot be posted.");

            var adjustment = await adjustmentRepository.GetByIdAsync(adjustmentId);
            if (adjustment is null)
                throw new InvalidOperationException("Adjustment not found.");

            await adjustmentRepository.PostAsync(adjustmentId, userId.ToString());
            await CreateLedgerEntryForAdjustmentAsync(adjustment, userId);

            return true;
        }

        public async Task<bool> DeleteAdjustmentAsync(int adjustmentId, string deletedBy)
        {
            var adjustment = await adjustmentRepository.GetByIdAsync(adjustmentId);
            if (adjustment is null)
                throw new InvalidOperationException("Adjustment not found.");

            if (adjustment.Status != "Draft")
                throw new InvalidOperationException("Only Draft adjustments can be deleted.");

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
                    throw new InvalidOperationException(
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
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            return await ledgerRepository.AddAsync(ledger);
        }
    }
}

