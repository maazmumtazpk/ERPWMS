using ERPWMS.Application.DTOs.Procurement;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Procurement
{
    public class CreateGrnValidator : AbstractValidator<CreateGrnRequest>
    {
        private readonly IGrnRepository _grnRepository;
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public CreateGrnValidator(
            IGrnRepository grnRepository,
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository)
        {
            _grnRepository = grnRepository;
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;

            // ? Purchase Order validation
            RuleFor(x => x.PurchaseId)
                .NotEmpty().WithMessage("Valid Purchase Order is required.")
                .MustAsync(PurchaseOrderExists).WithMessage("Purchase Order not found.");

            // ? Warehouse validation
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Valid Warehouse is required.");

            // ? Grn Number uniqueness
            RuleFor(x => x.GrnNo)
                .NotEmpty().WithMessage("Grn Number is required.")
                .MaximumLength(50).WithMessage("Grn Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueGrnNo).WithMessage("Grn Number already exists.");

            // ? Receipt Date validation
            RuleFor(x => x.ReceiptDate)
                .NotNull().WithMessage("Receipt Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Receipt Date cannot be in the future.");

            // ? Grn Details validation
            RuleFor(x => x.Grndetails)
                .NotNull().WithMessage("Grn Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one product is required.");

            // ? Each Grn Detail validation
            RuleForEach(x => x.Grndetails)
                .SetValidator(new CreateGrndetailValidator(_purchaseDetailRepository));

            // ? Validate total quantities against Purchase Order
            RuleFor(x => x)
                .MustAsync(ValidateQuantitiesAgainstPurchaseOrder)
                .WithMessage("Grn quantities exceed Purchase Order quantities.");
        }

        private async Task<bool> PurchaseOrderExists(int purchaseId, CancellationToken cancellationToken)
        {
            var purchase = await _purchaseMasterRepository.GetByIdAsync(purchaseId, 1);
            return purchase != null && purchase.IsDeleted != true;
        }

        private async Task<bool> BeUniqueGrnNo(string grnNo, CancellationToken cancellationToken)
        {
            return await _grnRepository.IsGrnNoUniqueAsync(grnNo, 1, null);
        }

        private async Task<bool> ValidateQuantitiesAgainstPurchaseOrder(CreateGrnRequest request, CancellationToken cancellationToken)
        {
            foreach (var detail in request.Grndetails)
            {
                var purchaseDetail = await _purchaseDetailRepository.GetByIdAsync(detail.PurchaseDetailId);
                if (purchaseDetail == null)
                    return false;

                // Check if Grn quantity exceeds PO quantity
                if (detail.AcceptedQty + (detail.RejectedQty ?? 0) > purchaseDetail.Quantity)
                    return false;

                // Check if total received (including previous Grns) exceeds PO quantity
                var totalReceived = await _purchaseDetailRepository.GetTotalReceivedQtyAsync(purchaseDetail.PurchaseId);
                if (totalReceived + detail.AcceptedQty > purchaseDetail.Quantity)
                    return false;
            }
            return true;
        }
    }

    public class CreateGrndetailValidator : AbstractValidator<CreateGrndetailRequest>
    {
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public CreateGrndetailValidator(IPurchaseDetailRepository purchaseDetailRepository)
        {
            _purchaseDetailRepository = purchaseDetailRepository;

            RuleFor(x => x.PurchaseDetailId)
                .NotEmpty().WithMessage("Valid Purchase Detail is required.")
                .MustAsync(PurchaseDetailExists).WithMessage("Purchase Detail not found.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.BinId)
                .NotEmpty().WithMessage("Valid Bin is required.");

            // ? Received Quantity must be >= Accepted Quantity
            RuleFor(x => x)
                .Must(x => x.ReceivedQty >= x.AcceptedQty)
                .WithMessage("Received Quantity must be greater than or equal to Accepted Quantity.");

            // ? Rejected Quantity must be <= Received Quantity
            RuleFor(x => x)
                .Must(x => !x.RejectedQty.HasValue || x.RejectedQty.Value <= x.ReceivedQty - x.AcceptedQty)
                .WithMessage("Rejected Quantity cannot exceed (Received - Accepted) quantity.");

            RuleFor(x => x.ReceivedQty)
                .GreaterThanOrEqualTo(0).WithMessage("Received Quantity cannot be negative.");

            RuleFor(x => x.AcceptedQty)
                .GreaterThanOrEqualTo(0).WithMessage("Accepted Quantity cannot be negative.");

            RuleFor(x => x.ExpiryDate)
                .Must(expiry => !expiry.HasValue || expiry > DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Expiry Date must be in the future.")
                .When(x => x.ExpiryDate.HasValue);

            RuleFor(x => x.RejectionReason)
                .MaximumLength(500).WithMessage("Rejection Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.RejectionReason));
        }

        private async Task<bool> PurchaseDetailExists(int purchaseDetailId, CancellationToken cancellationToken)
        {
            var detail = await _purchaseDetailRepository.GetByIdAsync(purchaseDetailId);
            return detail != null && detail.IsDeleted != true;
        }
    }
}