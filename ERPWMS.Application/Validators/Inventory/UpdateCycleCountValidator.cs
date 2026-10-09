using ERPWMS.Application.DTOs.Inventory;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Inventory
{
    public class UpdateCycleCountValidator : AbstractValidator<UpdateCycleCountRequest>
    {
        private readonly ICycleCountRepository _cycleCountRepository;

        private static readonly string[] AllowedStatuses =
        [
            "Draft",
            "Submitted",
            "Verified",
            "Rejected",
            "Posted",
            "Deleted"
        ];

        public UpdateCycleCountValidator(
            ICycleCountRepository cycleCountRepository)
        {
            _cycleCountRepository = cycleCountRepository;

            // =========================================================
            // CYCLE COUNT ID
            // =========================================================
            RuleFor(x => x.CycleCountId)
                .NotEmpty()
                .WithMessage("Valid Cycle Count ID is required.")

                .MustAsync(async (cycleCountId, cancellationToken) =>
                    await CycleCountExists(cycleCountId, cancellationToken))
                .WithMessage("Cycle Count not found.")

                .MustAsync(async (cycleCountId, cancellationToken) =>
                    await CanBeUpdated(cycleCountId, cancellationToken))
                .WithMessage("Only Draft cycle counts can be updated.");

            // =========================================================
            // WAREHOUSE
            // =========================================================
            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("Valid Warehouse is required.");

            // =========================================================
            // PRODUCT
            // =========================================================
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");

            // =========================================================
            // BIN
            // =========================================================
            RuleFor(x => x.BinId)
                .NotEmpty()
                .WithMessage("Valid Bin is required.");

            // =========================================================
            // STATUS
            // =========================================================
            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrWhiteSpace(status) ||
                    AllowedStatuses.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Submitted, Verified, Rejected, Posted, Deleted.");

            // =========================================================
            // DETAILS
            // =========================================================
            RuleFor(x => x.CycleCountDetails)
                .NotNull()
                .WithMessage("Cycle Count Details are required.")
                .Must(details => details != null && details.Any())
                .WithMessage("At least one cycle count detail is required.");

            // =========================================================
            // DETAIL VALIDATION
            // =========================================================
            RuleForEach(x => x.CycleCountDetails)
                .SetValidator(new UpdateCycleCountDetailValidator());
        }

        // =============================================================
        // CHECK CYCLE COUNT EXISTS
        // =============================================================
        private async Task<bool> CycleCountExists(
            int cycleCountId,
            CancellationToken cancellationToken)
        {
            if (cycleCountId <= 0)
                return false;

            var cycleCount =
                await _cycleCountRepository.GetByIdAsync(cycleCountId);

            return cycleCount != null;
        }

        // =============================================================
        // CHECK CYCLE COUNT CAN BE UPDATED
        // =============================================================
        private async Task<bool> CanBeUpdated(
            int cycleCountId,
            CancellationToken cancellationToken)
        {
            if (cycleCountId <= 0)
                return false;

            var cycleCount =
                await _cycleCountRepository.GetByIdAsync(cycleCountId);

            return cycleCount != null &&
                   cycleCount.Status == "Draft";
        }
    }

    // =================================================================
    // UPDATE CYCLE COUNT DETAIL VALIDATOR
    // =================================================================
    public class UpdateCycleCountDetailValidator
        : AbstractValidator<UpdateCycleCountDetailRequest>
    {
        public UpdateCycleCountDetailValidator()
        {
            // =========================================================
            // DETAIL ID
            // =========================================================
            RuleFor(x => x.CountDetailId)
                .NotEmpty()
                .WithMessage("Valid Count Detail ID is required.");

            // =========================================================
            // PRODUCT
            // =========================================================
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");

            // =========================================================
            // BIN
            // =========================================================
            RuleFor(x => x.BinId)
                .NotEmpty()
                .WithMessage("Valid Bin is required.");

            // =========================================================
            // SYSTEM QTY
            // =========================================================
            RuleFor(x => x.SystemQty)
                .GreaterThanOrEqualTo(0)
                .WithMessage(
                    "System Quantity cannot be negative.");

            // =========================================================
            // ACTUAL QTY
            // =========================================================
            RuleFor(x => x.ActualQty)
                .GreaterThanOrEqualTo(0)
                .WithMessage(
                    "Actual Quantity cannot be negative.");

            // =========================================================
            // VARIANCE = ACTUAL - SYSTEM
            // =========================================================
            RuleFor(x => x.Variance)
                .Equal(x => x.ActualQty - x.SystemQty)
                .WithMessage(
                    "Variance must equal ActualQty - SystemQty.");

            // =========================================================
            // REMARKS
            // =========================================================
            RuleFor(x => x.Remarks)
                .MaximumLength(500)
                .WithMessage(
                    "Remarks cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Remarks));
        }
    }
}
