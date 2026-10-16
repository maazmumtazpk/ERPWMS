using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Sales
{
    public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;

        public CreateInvoiceValidator(
            IInvoiceRepository invoiceRepository,
            ISalesDetailRepository salesDetailRepository)
        {
            _invoiceRepository = invoiceRepository;
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.SalesId)
                .NotEmpty().WithMessage("Valid Sales Order is required.");

            RuleFor(x => x.InvoiceNo)
                .NotEmpty().WithMessage("Invoice Number is required.")
                .MaximumLength(50).WithMessage("Invoice Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueInvoiceNo).WithMessage("Invoice Number already exists.");

            RuleFor(x => x.InvoiceDate)
                .NotNull().WithMessage("Invoice Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Invoice Date cannot be in the future.");

            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100.")
                .When(x => x.Discount.HasValue);

            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Tax Amount cannot be negative.")
                .When(x => x.TaxAmount.HasValue);

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due Date is required.")
                .Must((request, date) => date.ToDateTime(TimeOnly.MinValue) >= request.InvoiceDate)
                .WithMessage("Due Date must be after Invoice Date.");

            RuleFor(x => x.InvoiceDetails)
                .NotNull().WithMessage("Invoice Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.InvoiceDetails)
                .SetValidator(new CreateInvoiceDetailValidator(_salesDetailRepository));
        }

        private async Task<bool> BeUniqueInvoiceNo(string invoiceNo, CancellationToken cancellationToken)
        {
            return await _invoiceRepository.IsInvoiceNoUniqueAsync(invoiceNo, 1, null);
        }
    }

    public class CreateInvoiceDetailValidator : AbstractValidator<CreateInvoiceDetailRequest>
    {
        private readonly ISalesDetailRepository _salesDetailRepository;

        public CreateInvoiceDetailValidator(ISalesDetailRepository salesDetailRepository)
        {
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.SalesDetailId)
                .NotEmpty().WithMessage("Valid Sales Detail is required.")
                .MustAsync(SalesDetailExists).WithMessage("Sales Detail not found.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity must be greater than 0.")
                .Must((request, qty) => qty <= GetSalesDetailShippedQty(request.SalesDetailId).Result)
                .WithMessage("Quantity cannot exceed shipped quantity.");

            RuleFor(x => x.UnitPrice)
                .NotEmpty().WithMessage("Unit Price must be greater than 0.");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Discount percent must be between 0 and 100.")
                .When(x => x.DiscountPercent.HasValue);

            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Tax Amount cannot be negative.")
                .When(x => x.TaxAmount.HasValue);
        }

        private async Task<bool> SalesDetailExists(int salesDetailId, CancellationToken cancellationToken)
        {
            var detail = await _salesDetailRepository.GetByIdAsync(salesDetailId);
            return detail != null && detail.IsDeleted != true;
        }

        private async Task<int> GetSalesDetailShippedQty(int salesDetailId)
        {
            var detail = await _salesDetailRepository.GetByIdAsync(salesDetailId);
            return detail?.ShippedQty ?? 0;
        }
    }
}

