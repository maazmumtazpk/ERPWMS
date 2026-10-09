
using ERPWMS.Application.DTOs.Procurement;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Procurement
{
    public class CreatePurchaseReturnValidator : AbstractValidator<CreatePurchaseReturnRequest>
    {
        private readonly IPurchaseReturnRepository _purchaseReturnRepository;
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;
        private readonly IGrndetailRepository _grnDetailRepository;

        public CreatePurchaseReturnValidator(
            IPurchaseReturnRepository purchaseReturnRepository,
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository,
            IGrndetailRepository grnDetailRepository)
        {
            _purchaseReturnRepository = purchaseReturnRepository;
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
            _grnDetailRepository = grnDetailRepository;

            // Purchase Order validation
            RuleFor(x => x.PurchaseId)
                .NotEmpty()
                .WithMessage("Valid Purchase Order is required.")
                .MustAsync(PurchaseOrderExists)
                .WithMessage("Purchase Order not found.");

            // Grn validation - optional
            RuleFor(x => x.GrnId)
                .MustAsync(GrnExists)
                .WithMessage("Grn not found.")
                .When(x => x.GrnId.HasValue);

            // Return Number validation
            RuleFor(x => x.ReturnNo)
                .NotEmpty()
                .WithMessage("Return Number is required.")
                .MaximumLength(50)
                .WithMessage("Return Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueReturnNo)
                .WithMessage("Return Number already exists.");

            // Return Date validation
            RuleFor(x => x.ReturnDate)
                .NotNull()
                .WithMessage("Return Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Return Date cannot be in the future.");

            // Return Reason validation
            RuleFor(x => x.ReturnReason)
                .NotEmpty()
                .WithMessage("Return Reason is required.")
                .MaximumLength(500)
                .WithMessage("Return Reason cannot exceed 500 characters.");

            // Return Details validation
            RuleFor(x => x.PurchaseReturnDetails)
                .NotNull()
                .WithMessage("Return Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one product is required.");

            // Each Return Detail validation
            RuleForEach(x => x.PurchaseReturnDetails)
                .SetValidator(
                    new CreatePurchaseReturnDetailValidator(
                        _purchaseDetailRepository,
                        _grnDetailRepository));

            // Validate quantity against received quantity
            RuleFor(x => x)
                .MustAsync(ValidateQuantitiesAgainstReceived)
                .WithMessage("Return quantities exceed received quantities.");
        }

        private async Task<bool> PurchaseOrderExists(
            int purchaseId,
            CancellationToken cancellationToken)
        {
            var purchase = await _purchaseMasterRepository
                .GetByIdAsync(purchaseId, 1);

            return purchase != null &&
                   purchase.IsDeleted != true;
        }

        private async Task<bool> GrnExists(
            int? grnId,
            CancellationToken cancellationToken)
        {
            if (!grnId.HasValue)
                return true;

            var grn = await _grnDetailRepository
                .GetByIdAsync(grnId.Value);

            return grn != null;
        }

        private async Task<bool> BeUniqueReturnNo(
            string returnNo,
            CancellationToken cancellationToken)
        {
            return await _purchaseReturnRepository
                .IsReturnNoUniqueAsync(returnNo, 1, null);
        }

        private async Task<bool> ValidateQuantitiesAgainstReceived(
            CreatePurchaseReturnRequest request,
            CancellationToken cancellationToken)
        {
            if (request.PurchaseReturnDetails == null ||
                request.PurchaseReturnDetails.Count == 0)
            {
                return true;
            }

            foreach (var detail in request.PurchaseReturnDetails)
            {
                // Purchase Detail must exist
                var purchaseDetail = await _purchaseDetailRepository
                    .GetByIdAsync(detail.PurchaseDetailId);

                if (purchaseDetail == null ||
                    purchaseDetail.IsDeleted == true)
                {
                    return false;
                }

                // Total accepted quantity received against this Purchase Detail
                var receivedQty = await _grnDetailRepository
                    .GetTotalAcceptedQtyByPurchaseDetailIdAsync(
                        detail.PurchaseDetailId);

                // Returned quantity cannot exceed received quantity
                if (detail.Quantity > receivedQty)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class CreatePurchaseReturnDetailValidator
        : AbstractValidator<CreatePurchaseReturnDetailRequest>
    {
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;
        private readonly IGrndetailRepository _grnDetailRepository;

        public CreatePurchaseReturnDetailValidator(
            IPurchaseDetailRepository purchaseDetailRepository,
            IGrndetailRepository grnDetailRepository)
        {
            _purchaseDetailRepository = purchaseDetailRepository;
            _grnDetailRepository = grnDetailRepository;

            RuleFor(x => x.PurchaseDetailId)
                .NotEmpty()
                .WithMessage("Valid Purchase Detail is required.")
                .MustAsync(PurchaseDetailExists)
                .WithMessage("Purchase Detail not found.");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.UnitPrice)
                .NotEmpty()
                .WithMessage("Unit Price must be greater than 0.");

            RuleFor(x => x.ExpiryDate)
                .Must(expiry =>
                    !expiry.HasValue ||
                    expiry >
                    DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Expiry Date must be in the future.")
                .When(x => x.ExpiryDate.HasValue);

            RuleFor(x => x.Quantity)
                .MustAsync(QuantityWithinReceivedLimit)
                .WithMessage("Return quantity exceeds received quantity.");
        }

        private async Task<bool> PurchaseDetailExists(
            int purchaseDetailId,
            CancellationToken cancellationToken)
        {
            var detail = await _purchaseDetailRepository
                .GetByIdAsync(purchaseDetailId);

            return detail != null &&
                   detail.IsDeleted != true;
        }

        private async Task<bool> QuantityWithinReceivedLimit(
            CreatePurchaseReturnDetailRequest request,
            int quantity,
            CancellationToken cancellationToken)
        {
            var receivedQty = await _grnDetailRepository
                .GetTotalAcceptedQtyByPurchaseDetailIdAsync(
                    request.PurchaseDetailId);

            return quantity <= receivedQty;
        }
    }
}

