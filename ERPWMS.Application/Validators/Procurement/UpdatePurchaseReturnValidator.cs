
using ERPWMS.Application.DTOs.Procurement;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Procurement
{
    public class UpdatePurchaseReturnValidator
        : AbstractValidator<UpdatePurchaseReturnRequest>
    {
        private readonly IPurchaseReturnRepository _purchaseReturnRepository;
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;
        private readonly IGrndetailRepository _grnDetailRepository;

        public UpdatePurchaseReturnValidator(
            IPurchaseReturnRepository purchaseReturnRepository,
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository,
            IGrndetailRepository grnDetailRepository)
        {
            _purchaseReturnRepository = purchaseReturnRepository;
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
            _grnDetailRepository = grnDetailRepository;

            // ============================================================
            // RETURN ID
            // ============================================================

            RuleFor(x => x.ReturnId)
                .NotEmpty()
                .WithMessage("Valid Return ID is required.");

            // ============================================================
            // PURCHASE ORDER
            // ============================================================

            RuleFor(x => x.PurchaseId)
                .NotEmpty()
                .WithMessage("Valid Purchase Order is required.")
                .MustAsync(PurchaseOrderExists)
                .WithMessage("Purchase Order not found.");

            // ============================================================
            // Grn - OPTIONAL
            // ============================================================

            RuleFor(x => x.GrnId)
                .MustAsync(GrnExists)
                .WithMessage("Grn not found.")
                .When(x => x.GrnId.HasValue);

            // ============================================================
            // RETURN NUMBER
            // ============================================================

            RuleFor(x => x.ReturnNo)
                .NotEmpty()
                .WithMessage("Return Number is required.")
                .MaximumLength(50)
                .WithMessage("Return Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueReturnNo)
                .WithMessage("Return Number already exists.");

            // ============================================================
            // RETURN DATE
            // ============================================================

            RuleFor(x => x.ReturnDate)
                .NotNull()
                .WithMessage("Return Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Return Date cannot be in the future.");

            // ============================================================
            // RETURN REASON
            // ============================================================

            RuleFor(x => x.ReturnReason)
                .NotEmpty()
                .WithMessage("Return Reason is required.")
                .MaximumLength(500)
                .WithMessage("Return Reason cannot exceed 500 characters.");

            // ============================================================
            // STATUS
            // ============================================================

            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    new[]
                    {
                        "Draft",
                        "Submitted",
                        "Approved",
                        "Rejected",
                        "Cancelled"
                    }.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Submitted, Approved, Rejected, Cancelled.");

            // ============================================================
            // RETURN DETAILS
            // ============================================================

            RuleFor(x => x.PurchaseReturnDetails)
                .NotNull()
                .WithMessage("Return Details are required.")
                .Must(details =>
                    details != null &&
                    details.Count > 0)
                .WithMessage("At least one product is required.");

            // ============================================================
            // EACH RETURN DETAIL
            //
            // PurchaseReturnDetails collection contains
            // CreatePurchaseReturnDetailRequest.
            //
            // Therefore use CreatePurchaseReturnDetailValidator.
            // It requires:
            // 1. IPurchaseDetailRepository
            // 2. IGrndetailRepository
            // ============================================================

            RuleForEach(x => x.PurchaseReturnDetails)
                .SetValidator(
                    new CreatePurchaseReturnDetailValidator(
                        _purchaseDetailRepository,
                        _grnDetailRepository));

            // ============================================================
            // QUANTITY VALIDATION
            // ============================================================

            RuleFor(x => x)
                .MustAsync(ValidateQuantitiesAgainstReceived)
                .WithMessage("Return quantities exceed received quantities.");
        }

        // ================================================================
        // PURCHASE ORDER EXISTS
        // ================================================================

        private async Task<bool> PurchaseOrderExists(
            int purchaseId,
            CancellationToken cancellationToken)
        {
            var purchase =
                await _purchaseMasterRepository.GetByIdAsync(
                    purchaseId,
                    1);

            return purchase != null &&
                   purchase.IsDeleted != true;
        }

        // ================================================================
        // Grn EXISTS
        // ================================================================

        private async Task<bool> GrnExists(
            int? grnId,
            CancellationToken cancellationToken)
        {
            if (!grnId.HasValue)
            {
                return true;
            }

            var grn =
                await _grnDetailRepository.GetByIdAsync(grnId.Value);

            return grn != null;
        }

        // ================================================================
        // RETURN NUMBER UNIQUE
        // ================================================================

        private async Task<bool> BeUniqueReturnNo(
            UpdatePurchaseReturnRequest request,
            string returnNo,
            CancellationToken cancellationToken)
        {
            return await _purchaseReturnRepository
                .IsReturnNoUniqueAsync(
                    returnNo,
                    1,
                    request.ReturnId);
        }

        // ================================================================
        // VALIDATE RETURN QUANTITY AGAINST RECEIVED QUANTITY
        // ================================================================

        private async Task<bool> ValidateQuantitiesAgainstReceived(
            UpdatePurchaseReturnRequest request,
            CancellationToken cancellationToken)
        {
            if (request.PurchaseReturnDetails == null ||
                request.PurchaseReturnDetails.Count == 0)
            {
                return false;
            }

            foreach (var detail in request.PurchaseReturnDetails)
            {
                var receivedQty =
                    await _grnDetailRepository
                        .GetTotalAcceptedQtyByPurchaseDetailIdAsync(
                            detail.PurchaseDetailId);

                if (detail.Quantity > receivedQty)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

