
using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Sales
{
    public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceRequest>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;

        public UpdateInvoiceValidator(
            IInvoiceRepository invoiceRepository,
            ISalesDetailRepository salesDetailRepository)
        {
            _invoiceRepository = invoiceRepository;
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithMessage("Valid Invoice ID is required.");

            RuleFor(x => x.SalesId)
                .NotEmpty()
                .WithMessage("Valid Sales Order is required.");

            RuleFor(x => x.InvoiceNo)
                .NotEmpty()
                .WithMessage("Invoice Number is required.")
                .MaximumLength(50)
                .WithMessage("Invoice Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueInvoiceNo)
                .WithMessage("Invoice Number already exists.");

            RuleFor(x => x.InvoiceDate)
                .NotNull()
                .WithMessage("Invoice Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Invoice Date cannot be in the future.");

            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100)
                .WithMessage("Discount must be between 0 and 100.")
                .When(x => x.Discount.HasValue);

            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tax Amount cannot be negative.")
                .When(x => x.TaxAmount.HasValue);

            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due Date is required.")
                .Must((request, date) =>
                    date.ToDateTime(TimeOnly.MinValue) >= request.InvoiceDate)
                .WithMessage("Due Date must be after Invoice Date.");

            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    new[]
                    {
                        "Draft",
                        "Posted",
                        "Paid",
                        "Cancelled"
                    }.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Posted, Paid, Cancelled.");

            RuleFor(x => x.InvoiceDetails)
                .NotNull()
                .WithMessage("Invoice Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            // IMPORTANT:
            // UpdateInvoiceRequest.InvoiceDetails is
            // List<CreateInvoiceDetailRequest>
            // Therefore CreateInvoiceDetailValidator must be used here.
            RuleForEach(x => x.InvoiceDetails)
                .SetValidator(
                    new CreateInvoiceDetailValidator(_salesDetailRepository));
        }

        private async Task<bool> BeUniqueInvoiceNo(
            UpdateInvoiceRequest request,
            string invoiceNo,
            CancellationToken cancellationToken)
        {
            return await _invoiceRepository.IsInvoiceNoUniqueAsync(
                invoiceNo,
                1,
                request.InvoiceId);
        }
    }
}


