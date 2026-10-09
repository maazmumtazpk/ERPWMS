
using ERPWMS.Application.DTOs.Inventory;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Inventory
{
    public class UpdateStockAdjustmentValidator
        : AbstractValidator<UpdateStockAdjustmentRequest>
    {
        private readonly IStockAdjustmentRepository _adjustmentRepository;

        private static readonly string[] AllowedStatuses =
            ["Draft", "Submitted", "Approved", "Rejected", "Posted", "Deleted"];

        public UpdateStockAdjustmentValidator(
            IStockAdjustmentRepository adjustmentRepository)
        {
            _adjustmentRepository = adjustmentRepository;

            // =====================================================
            // Adjustment ID Validation
            // =====================================================

            RuleFor(x => x.AdjustmentId)
                .NotEmpty()
                .WithMessage("Valid Adjustment ID is required.");

            RuleFor(x => x.AdjustmentId)
                .MustAsync(AdjustmentExists)
                .When(x => x.AdjustmentId > 0)
                .WithMessage("Adjustment not found.");

            RuleFor(x => x.AdjustmentId)
                .MustAsync(CanBeUpdated)
                .When(x => x.AdjustmentId > 0)
                .WithMessage("Only Draft adjustments can be updated.");


            // =====================================================
            // Warehouse Validation
            // =====================================================

            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("Valid Warehouse is required.");


            // =====================================================
            // Product Validation
            // =====================================================

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");


            // =====================================================
            // Bin Validation
            // =====================================================

            RuleFor(x => x.BinId)
                .NotEmpty()
                .WithMessage("Valid Bin is required.");


            // =====================================================
            // Adjustment Type Validation
            // =====================================================

            RuleFor(x => x.AdjustmentType)
                .NotEmpty()
                .WithMessage("Adjustment Type is required.")
                .MaximumLength(20)
                .WithMessage("Adjustment Type cannot exceed 20 characters.")
                .Must(type =>
                    type == "Increase" ||
                    type == "Decrease")
                .WithMessage(
                    "Adjustment Type must be 'Increase' or 'Decrease'."
                );


            // =====================================================
            // Quantity Validation
            // =====================================================

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity must be greater than 0.");


            // =====================================================
            // Reason Validation
            // =====================================================

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason is required.")
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters.");


            // =====================================================
            // Status Validation
            // =====================================================

            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    AllowedStatuses.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Submitted, Approved, Rejected, Posted, Deleted."
                );


            // =====================================================
            // Adjustment Details Validation
            // =====================================================

            RuleFor(x => x.AdjustmentDetails)
                .NotNull()
                .WithMessage("Adjustment Details are required.");

            RuleFor(x => x.AdjustmentDetails)
                .Must(details =>
                    details != null &&
                    details.Count > 0)
                .WithMessage(
                    "At least one adjustment detail is required."
                );


            // =====================================================
            // Each Adjustment Detail
            // =====================================================

            RuleForEach(x => x.AdjustmentDetails)
                .SetValidator(
                    new UpdateStockAdjustmentDetailValidator()
                );
        }


        // =========================================================
        // Adjustment Exists
        // =========================================================

        private async Task<bool> AdjustmentExists(
            int adjustmentId,
            CancellationToken cancellationToken)
        {
            var adjustment =
                await _adjustmentRepository.GetByIdAsync(adjustmentId);

            return adjustment != null;
        }


        // =========================================================
        // Adjustment Can Be Updated
        // =========================================================

        private async Task<bool> CanBeUpdated(
            int adjustmentId,
            CancellationToken cancellationToken)
        {
            var adjustment =
                await _adjustmentRepository.GetByIdAsync(adjustmentId);

            return adjustment != null &&
                   adjustment.Status == "Draft";
        }
    }


    // =============================================================
    // UPDATE STOCK ADJUSTMENT DETAIL VALIDATOR
    // =============================================================

    public class UpdateStockAdjustmentDetailValidator
        : AbstractValidator<CreateStockAdjustmentDetailRequest>
    {
        public UpdateStockAdjustmentDetailValidator()
        {
            // =====================================================
            // Product
            // =====================================================

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");


            // =====================================================
            // Bin
            // =====================================================

            RuleFor(x => x.BinId)
                .NotEmpty()
                .WithMessage("Valid Bin is required.");


            // =====================================================
            // Quantity
            // =====================================================

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity must be greater than 0.");


            // =====================================================
            // Batch Number
            // =====================================================

            RuleFor(x => x.BatchNo)
                .MaximumLength(50)
                .WithMessage("Batch No cannot exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.BatchNo));
        }
    }
}


