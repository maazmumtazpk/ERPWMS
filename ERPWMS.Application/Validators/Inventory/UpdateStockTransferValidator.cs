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
    public class UpdateStockTransferValidator
        : AbstractValidator<UpdateStockTransferRequest>
    {
        private readonly IStockTransferRepository _transferRepository;

        public UpdateStockTransferValidator(
            IStockTransferRepository transferRepository)
        {
            _transferRepository = transferRepository;

            // =====================================================
            // TRANSFER ID
            // =====================================================

            RuleFor(x => x.TransferId)
                .NotEmpty()
                .WithMessage("Valid Transfer ID is required.")
                .MustAsync(TransferExists)
                .WithMessage("Transfer not found.")
                .MustAsync(CanBeUpdated)
                .WithMessage("Only Draft transfers can be updated.");

            // =====================================================
            // FROM WAREHOUSE
            // =====================================================

            RuleFor(x => x.FromWarehouseId)
                .NotEmpty()
                .WithMessage("Valid From Warehouse is required.");

            // =====================================================
            // TO WAREHOUSE
            // =====================================================

            RuleFor(x => x.ToWarehouseId)
                .NotEmpty()
                .WithMessage("Valid To Warehouse is required.");

            // =====================================================
            // WAREHOUSE DIFFERENCE
            // =====================================================

            RuleFor(x => x)
                .Must(x => x.FromWarehouseId != x.ToWarehouseId)
                .WithMessage(
                    "From Warehouse and To Warehouse cannot be the same.");

            // =====================================================
            // PRODUCT
            // =====================================================

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");

            // =====================================================
            // QUANTITY
            // =====================================================

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity must be greater than 0.");

            // =====================================================
            // TRANSFER NUMBER
            // =====================================================

            RuleFor(x => x.TransferNo)
                .NotEmpty()
                .WithMessage("Transfer Number is required.")
                .MaximumLength(50)
                .WithMessage(
                    "Transfer Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueTransferNo)
                .WithMessage("Transfer Number already exists.");

            // =====================================================
            // TRANSFER DATE
            // =====================================================

            RuleFor(x => x.TransferDate)
                .NotNull()
                .WithMessage("Transfer Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Transfer Date cannot be in the future.");

            // =====================================================
            // STATUS
            // =====================================================

            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    new[]
                    {
                        "Draft",
                        "Submitted",
                        "Approved",
                        "Rejected",
                        "InTransit",
                        "Completed",
                        "Deleted"
                    }.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Submitted, Approved, Rejected, InTransit, Completed, Deleted.");

            // =====================================================
            // TRANSFER DETAILS
            // =====================================================

            RuleFor(x => x.TransferDetails)
                .NotNull()
                .WithMessage("At least one transfer detail is required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one transfer detail is required.");

            // =====================================================
            // EACH TRANSFER DETAIL
            // =====================================================

            RuleForEach(x => x.TransferDetails)
                .SetValidator(new UpdateStockTransferDetailValidator());
        }

        // =========================================================
        // TRANSFER EXISTS
        // =========================================================

        private async Task<bool> TransferExists(
            int transferId,
            CancellationToken cancellationToken)
        {
            var transfer =
                await _transferRepository.GetByIdAsync(transferId);

            return transfer != null;
        }

        // =========================================================
        // CAN BE UPDATED
        // =========================================================

        private async Task<bool> CanBeUpdated(
            int transferId,
            CancellationToken cancellationToken)
        {
            var transfer =
                await _transferRepository.GetByIdAsync(transferId);

            return transfer != null &&
                   transfer.Status == "Draft";
        }

        // =========================================================
        // UNIQUE TRANSFER NUMBER
        // =========================================================

        private async Task<bool> BeUniqueTransferNo(
            UpdateStockTransferRequest request,
            string transferNo,
            CancellationToken cancellationToken)
        {
            return await _transferRepository
                .IsTransferNoUniqueAsync(
                    transferNo,
                    request.TransferId);
        }
    }

    // =============================================================
    // UPDATE STOCK TRANSFER DETAIL VALIDATOR
    // =============================================================

    public class UpdateStockTransferDetailValidator
        : AbstractValidator<CreateStockTransferDetailRequest>
    {
        public UpdateStockTransferDetailValidator()
        {
            // =====================================================
            // PRODUCT
            // =====================================================

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");

            // =====================================================
            // FROM BIN
            // =====================================================

            RuleFor(x => x.FromBinId)
                .NotEmpty()
                .WithMessage("Valid From Bin is required.");

            // =====================================================
            // TO BIN
            // =====================================================

            RuleFor(x => x.ToBinId)
                .NotEmpty()
                .WithMessage("Valid To Bin is required.");

            // =====================================================
            // QUANTITY
            // =====================================================

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity must be greater than 0.");

            // =====================================================
            // FROM BIN != TO BIN
            // =====================================================

            RuleFor(x => x.FromBinId)
                .NotEqual(x => x.ToBinId)
                .WithMessage(
                    "From Bin and To Bin cannot be the same.");

            // =====================================================
            // BATCH NUMBER
            // =====================================================

            RuleFor(x => x.BatchNo)
                .MaximumLength(50)
                .WithMessage(
                    "Batch No cannot exceed 50 characters.")
                .When(x =>
                    !string.IsNullOrEmpty(x.BatchNo));
        }
    }
}
