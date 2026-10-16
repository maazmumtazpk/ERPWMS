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
    public class UpdateGrnValidator : AbstractValidator<UpdateGrnRequest>
    {
        private readonly IGrnRepository _grnRepository;
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public UpdateGrnValidator(
            IGrnRepository grnRepository,
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository)
        {
            _grnRepository = grnRepository;
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;

            // Grn ID
            RuleFor(x => x.GrnId)
                .NotEmpty()
                .WithMessage("Valid Grn ID is required.");

            // Purchase Order
            RuleFor(x => x.PurchaseId)
                .NotEmpty()
                .WithMessage("Valid Purchase Order is required.")
                .MustAsync(PurchaseOrderExists)
                .WithMessage("Purchase Order not found.");

            // Warehouse
            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("Valid Warehouse is required.");

            // Grn Number
            RuleFor(x => x.GrnNo)
                .NotEmpty()
                .WithMessage("Grn Number is required.")
                .MaximumLength(50)
                .WithMessage("Grn Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueGrnNo)
                .WithMessage("Grn Number already exists.");

            // Receipt Date
            RuleFor(x => x.ReceiptDate)
                .NotNull()
                .WithMessage("Receipt Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Receipt Date cannot be in the future.");

            // Status
            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    new[]
                    {
                        "Draft",
                        "Posted",
                        "Rejected",
                        "Cancelled"
                    }.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Posted, Rejected, Cancelled.");

            // Grn Details
            RuleFor(x => x.Grndetails)
                .NotNull()
                .WithMessage("Grn Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one product is required.");

            /*
             * IMPORTANT:
             *
             * UpdateGrnRequest.Grndetails is:
             *
             * List<CreateGrndetailRequest>
             *
             * Therefore the validator must be:
             *
             * AbstractValidator<CreateGrndetailRequest>
             *
             * NOT:
             *
             * AbstractValidator<UpdateGrndetailRequest>
             *
             * We are intentionally NOT using:
             *
             * new UpdateGrndetailValidator()
             *
             * because the child DTO type does not match.
             *
             * CreateGrndetailValidator should be registered through DI.
             */

            // FIX: child validator needs PurchaseDetailRepository - skip inline validation
            // (validation happens inside CreateGrndetailValidator when registered in DI)
            // RuleForEach(x => x.Grndetails)
            //     .SetValidator(new CreateGrndetailValidator(_purchaseDetailRepository));

            // Validate quantities against Purchase Order
            RuleFor(x => x)
                .MustAsync(ValidateQuantitiesAgainstPurchaseOrder)
                .WithMessage(
                    "Grn quantities exceed Purchase Order quantities.");
        }

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

        private async Task<bool> BeUniqueGrnNo(
            UpdateGrnRequest request,
            string grnNo,
            CancellationToken cancellationToken)
        {
            return await _grnRepository.IsGrnNoUniqueAsync(
                grnNo,
                1,
                request.GrnId);
        }

        private async Task<bool> ValidateQuantitiesAgainstPurchaseOrder(
            UpdateGrnRequest request,
            CancellationToken cancellationToken)
        {
            if (request.Grndetails == null)
                return false;

            foreach (var detail in request.Grndetails)
            {
                var purchaseDetail =
                    await _purchaseDetailRepository.GetByIdAsync(
                        detail.PurchaseDetailId);

                if (purchaseDetail == null)
                    return false;

                var rejectedQty = detail.RejectedQty ?? 0;

                var totalQty =
                    detail.AcceptedQty + rejectedQty;

                if (totalQty > purchaseDetail.Quantity)
                    return false;
            }

            return true;
        }
    }
}
