
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
    public class UpdateDispatchValidator : AbstractValidator<UpdateDispatchRequest>
    {
        private readonly IDispatchMasterRepository _dispatchMasterRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;

        public UpdateDispatchValidator(
            IDispatchMasterRepository dispatchMasterRepository,
            ISalesDetailRepository salesDetailRepository)
        {
            _dispatchMasterRepository = dispatchMasterRepository;
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.DispatchId)
                .NotEmpty()
                .WithMessage("Valid Dispatch ID is required.");

            RuleFor(x => x.SalesId)
                .NotEmpty()
                .WithMessage("Valid Sales Order is required.");

            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("Valid Warehouse is required.");

            RuleFor(x => x.DispatchNo)
                .NotEmpty()
                .WithMessage("Dispatch Number is required.")
                .MaximumLength(50)
                .WithMessage("Dispatch Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueDispatchNo)
                .WithMessage("Dispatch Number already exists.");

            RuleFor(x => x.DispatchDate)
                .NotNull()
                .WithMessage("Dispatch Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Dispatch Date cannot be in the future.");

            RuleFor(x => x.CarrierName)
                .NotEmpty()
                .WithMessage("Carrier Name is required.")
                .MaximumLength(100)
                .WithMessage("Carrier Name cannot exceed 100 characters.");

            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    new[]
                    {
                        "Draft",
                        "Dispatched",
                        "Delivered",
                        "Cancelled"
                    }.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Dispatched, Delivered, Cancelled.");

            RuleFor(x => x.DispatchDetails)
                .NotNull()
                .WithMessage("Dispatch Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            // DispatchDetails ka type:
            // List<CreateDispatchDetailRequest>
            //
            // Isliye CreateDispatchDetailValidator use hoga.
            RuleForEach(x => x.DispatchDetails)
                .SetValidator(
                    new CreateDispatchDetailValidator(_salesDetailRepository));
        }

        private async Task<bool> BeUniqueDispatchNo(
            UpdateDispatchRequest request,
            string dispatchNo,
            CancellationToken cancellationToken)
        {
            return await _dispatchMasterRepository.IsDispatchNoUniqueAsync(
                dispatchNo,
                1,
                request.DispatchId);
        }
    }
}


