
using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Sales
{
    public class CreateSalesReturnValidator : AbstractValidator<CreateSalesReturnRequest>
    {
        private readonly ISalesReturnRepository _salesReturnRepository;
        private readonly IInvoiceDetailRepository _invoiceDetailRepository;

        public CreateSalesReturnValidator(
            ISalesReturnRepository salesReturnRepository,
            IInvoiceDetailRepository invoiceDetailRepository)
        {
            _salesReturnRepository = salesReturnRepository;
            _invoiceDetailRepository = invoiceDetailRepository;

            RuleFor(x => x.SalesId)
                .NotEmpty()
                .WithMessage("Valid Sales Order is required.");

            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithMessage("Valid Invoice is required.");

            RuleFor(x => x.ReturnNo)
                .NotEmpty()
                .WithMessage("Return Number is required.")
                .MaximumLength(50)
                .WithMessage("Return Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueReturnNo)
                .WithMessage("Return Number already exists.");

            RuleFor(x => x.ReturnDate)
                .NotNull()
                .WithMessage("Return Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Return Date cannot be in the future.");

            RuleFor(x => x.ReturnReason)
                .NotEmpty()
                .WithMessage("Return Reason is required.")
                .MaximumLength(500)
                .WithMessage("Return Reason cannot exceed 500 characters.");

            RuleFor(x => x.SalesReturnDetails)
                .NotNull()
                .WithMessage("Return Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.SalesReturnDetails)
                .SetValidator(
                    new CreateSalesReturnDetailValidator(
                        _invoiceDetailRepository));
        }

        private async Task<bool> BeUniqueReturnNo(
            string returnNo,
            CancellationToken cancellationToken)
        {
            return await _salesReturnRepository
                .IsReturnNoUniqueAsync(returnNo, 1, null);
        }
    }

    public class CreateSalesReturnDetailValidator
        : AbstractValidator<CreateSalesReturnDetailRequest>
    {
        private readonly IInvoiceDetailRepository _invoiceDetailRepository;

        public CreateSalesReturnDetailValidator(
            IInvoiceDetailRepository invoiceDetailRepository)
        {
            _invoiceDetailRepository = invoiceDetailRepository;

            RuleFor(x => x.InvoiceDetailId)
                .NotEmpty()
                .WithMessage("Valid Invoice Detail is required.")
                .MustAsync(InvoiceDetailExists)
                .WithMessage("Invoice Detail not found.");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Valid Product is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.UnitPrice)
                .NotEmpty()
                .WithMessage("Unit Price must be greater than 0.");
        }

        private async Task<bool> InvoiceDetailExists(
            int invoiceDetailId,
            CancellationToken cancellationToken)
        {
            var detail = await _invoiceDetailRepository
                .GetByIdAsync(invoiceDetailId);

            return detail != null &&
                   detail.IsDeleted != true;
        }
    }
}


