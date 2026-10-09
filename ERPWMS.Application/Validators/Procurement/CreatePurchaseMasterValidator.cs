using ERPWMS.Application.DTOs.Procurement;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Procurement
{
    public class CreatePurchaseMasterValidator : AbstractValidator<CreatePurchaseMasterRequest>
    {
        private readonly IPurchaseMasterRepository _purchaseMasterRepository;
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public CreatePurchaseMasterValidator(
            IPurchaseMasterRepository purchaseMasterRepository,
            IPurchaseDetailRepository purchaseDetailRepository)
        {
            _purchaseMasterRepository = purchaseMasterRepository;
            _purchaseDetailRepository = purchaseDetailRepository;

            // ? Supplier validation
            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Valid Supplier is required.");

            // ? Warehouse validation
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Valid Warehouse is required.");

            // ? Purchase Order Number uniqueness
            RuleFor(x => x.PurchaseOrderNo)
                .NotEmpty().WithMessage("Purchase Order Number is required.")
                .MaximumLength(50).WithMessage("Purchase Order Number cannot exceed 50 characters.")
                .MustAsync(BeUniquePurchaseOrderNo).WithMessage("Purchase Order Number already exists.");

            // ? Order Date validation
            RuleFor(x => x.OrderDate)
                .NotNull().WithMessage("Order Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Order Date cannot be in the future.");

            // ? Expected Delivery Date validation (must be after Order Date)
            RuleFor(x => x)
                .Must(x => !x.ExpectedDeliveryDate.HasValue ||
                           x.OrderDate.HasValue && x.ExpectedDeliveryDate.Value.ToDateTime(TimeOnly.MinValue) >= x.OrderDate.Value)
                .WithMessage("Expected Delivery Date must be after Order Date.");

            // ? Discount validation
            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100.")
                .When(x => x.Discount.HasValue);

            // ? Tax validation
            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Tax Amount cannot be negative.")
                .When(x => x.TaxAmount.HasValue);

            // ? Purchase Details validation
            RuleFor(x => x.PurchaseDetails)
                .NotNull().WithMessage("Purchase Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one product is required.");

            // ? Each Purchase Detail validation
            RuleForEach(x => x.PurchaseDetails)
                .SetValidator(new CreatePurchaseDetailValidator());
        }

        private async Task<bool> BeUniquePurchaseOrderNo(string purchaseOrderNo, CancellationToken cancellationToken)
        {
            // For create, companyId will be passed from service/controller
            // We'll assume companyId=1 for now; actual companyId will be set in service
            return await _purchaseMasterRepository.IsPurchaseOrderNoUniqueAsync(purchaseOrderNo, 1, null);
        }
    }

    public class CreatePurchaseDetailValidator : AbstractValidator<CreatePurchaseDetailRequest>
    {
        public CreatePurchaseDetailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.UnitPrice)
                .NotEmpty().WithMessage("Unit Price must be greater than 0.");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Discount percent must be between 0 and 100.")
                .When(x => x.DiscountPercent.HasValue);

            // Expiry Date should be in future if provided
            RuleFor(x => x.ExpiryDate)
                .Must(expiry => !expiry.HasValue || expiry > DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Expiry Date must be in the future.")
                .When(x => x.ExpiryDate.HasValue);
        }
    }
}