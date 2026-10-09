
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
    public class UpdateSalesReturnValidator : AbstractValidator<UpdateSalesReturnRequest>
    {
        private readonly ISalesReturnRepository _salesReturnRepository;
        private readonly IInvoiceDetailRepository _invoiceDetailRepository;

        public UpdateSalesReturnValidator(
            ISalesReturnRepository salesReturnRepository,
            IInvoiceDetailRepository invoiceDetailRepository)
        {
            _salesReturnRepository = salesReturnRepository;
            _invoiceDetailRepository = invoiceDetailRepository;

            RuleFor(x => x.ReturnId)
                .NotEmpty()
                .WithMessage("Valid Return ID is required.");

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

            RuleFor(x => x.SalesReturnDetails)
                .NotNull()
                .WithMessage("Return Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            // SalesReturnDetails is List<CreateSalesReturnDetailRequest>
            // and CreateSalesReturnDetailValidator requires
            // IInvoiceDetailRepository.
            RuleForEach(x => x.SalesReturnDetails)
                .SetValidator(
                    new CreateSalesReturnDetailValidator(
                        _invoiceDetailRepository));
        }

        private async Task<bool> BeUniqueReturnNo(
            UpdateSalesReturnRequest request,
            string returnNo,
            CancellationToken cancellationToken)
        {
            return await _salesReturnRepository.IsReturnNoUniqueAsync(
                returnNo,
                1,
                request.ReturnId);
        }
    }
}


