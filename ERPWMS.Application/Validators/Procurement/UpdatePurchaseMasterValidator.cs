
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
    public class UpdatePurchaseMasterValidator
        : AbstractValidator<UpdatePurchaseMasterRequest>
    {
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;

        public UpdatePurchaseMasterValidator(
            IPurchaseMasterRepository purchaseMasterRepository)
        {
            _purchaseMasterRepository = purchaseMasterRepository;

            // Purchase ID validation
            RuleFor(x => x.PurchaseId)
                .NotEmpty()
                .WithMessage("Valid Purchase ID is required.");

            // Supplier validation
            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("Valid Supplier is required.");

            // Warehouse validation
            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("Valid Warehouse is required.");

            // Purchase Order Number validation
            RuleFor(x => x.PurchaseOrderNo)
                .NotEmpty()
                .WithMessage("Purchase Order Number is required.")
                .MaximumLength(50)
                .WithMessage("Purchase Order Number cannot exceed 50 characters.")
                .MustAsync(BeUniquePurchaseOrderNo)
                .WithMessage("Purchase Order Number already exists.");

            // Order Date validation
            RuleFor(x => x.OrderDate)
                .NotNull()
                .WithMessage("Order Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Order Date cannot be in the future.");

            // Expected Delivery Date validation
            RuleFor(x => x)
                .Must(x =>
                    !x.ExpectedDeliveryDate.HasValue ||
                    !x.OrderDate.HasValue ||
                    x.ExpectedDeliveryDate.Value.ToDateTime(TimeOnly.MinValue)
                    >= x.OrderDate.Value)
                .WithMessage("Expected Delivery Date must be after Order Date.");

            // Discount validation
            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100)
                .WithMessage("Discount must be between 0 and 100.")
                .When(x => x.Discount.HasValue);

            // Tax validation
            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tax Amount cannot be negative.")
                .When(x => x.TaxAmount.HasValue);

            // Status validation
            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    new[]
                    {
                        "Draft",
                        "Submitted",
                        "Approved",
                        "Rejected",
                        "Completed",
                        "Cancelled"
                    }.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Submitted, Approved, Rejected, Completed, Cancelled.");

            // Purchase Details validation
            RuleFor(x => x.PurchaseDetails)
                .NotNull()
                .WithMessage("Purchase Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one product is required.");

            // IMPORTANT:
            // PurchaseDetails ka type CreatePurchaseDetailRequest hai.
            // Isliye existing CreatePurchaseDetailValidator use hoga.
            //
            // CreatePurchaseDetailValidator ko is file mein dobara
            // define NAHI karna.
            RuleForEach(x => x.PurchaseDetails)
                .SetValidator(new CreatePurchaseDetailValidator());
        }

        private async Task<bool> BeUniquePurchaseOrderNo(
            UpdatePurchaseMasterRequest request,
            string purchaseOrderNo,
            CancellationToken cancellationToken)
        {
            return await _purchaseMasterRepository
                .IsPurchaseOrderNoUniqueAsync(
                    purchaseOrderNo,
                    1,
                    request.PurchaseId);
        }
    }
}

