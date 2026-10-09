using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Sales
{
    public class CreateSalesMasterValidator : AbstractValidator<CreateSalesMasterRequest>
    {
        private readonly ISalesMasterRepository _salesMasterRepository;

        public CreateSalesMasterValidator(ISalesMasterRepository salesMasterRepository)
        {
            _salesMasterRepository = salesMasterRepository;

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Valid Customer is required.");

            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Valid Warehouse is required.");

            RuleFor(x => x.SalesOrderNo)
                .NotEmpty().WithMessage("Sales Order Number is required.")
                .MaximumLength(50).WithMessage("Sales Order Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueSalesOrderNo).WithMessage("Sales Order Number already exists.");

            RuleFor(x => x.OrderDate)
                .NotNull().WithMessage("Order Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Order Date cannot be in the future.");

            RuleFor(x => x.DeliveryDate)
                .Must((request, date) => !date.HasValue || request.OrderDate.HasValue && date.Value.ToDateTime(TimeOnly.MinValue) >= request.OrderDate.Value)
                .WithMessage("Delivery Date must be after Order Date.");

            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100.")
                .When(x => x.Discount.HasValue);

            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Tax Amount cannot be negative.")
                .When(x => x.TaxAmount.HasValue);

            RuleFor(x => x.SalesDetails)
                .NotNull().WithMessage("Sales Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one product is required.");

            RuleForEach(x => x.SalesDetails)
                .SetValidator(new CreateSalesDetailValidator());
        }

        private async Task<bool> BeUniqueSalesOrderNo(string orderNo, CancellationToken cancellationToken)
        {
            return await _salesMasterRepository.IsOrderNoUniqueAsync(orderNo, 1, null);
        }
    }

    public class CreateSalesDetailValidator : AbstractValidator<CreateSalesDetailRequest>
    {
        public CreateSalesDetailValidator()
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
        }
    }
}
