using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Sales
{
    public class CreateDispatchValidator : AbstractValidator<CreateDispatchRequest>
    {
        private readonly IDispatchMasterRepository _dispatchMasterRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;

        public CreateDispatchValidator(
            IDispatchMasterRepository dispatchMasterRepository,
            ISalesDetailRepository salesDetailRepository)
        {
            _dispatchMasterRepository = dispatchMasterRepository;
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.SalesId)
                .NotEmpty().WithMessage("Valid Sales Order is required.");

            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Valid Warehouse is required.");

            RuleFor(x => x.DispatchNo)
                .NotEmpty().WithMessage("Dispatch Number is required.")
                .MaximumLength(50).WithMessage("Dispatch Number cannot exceed 50 characters.")
                .MustAsync(BeUniqueDispatchNo).WithMessage("Dispatch Number already exists.");

            RuleFor(x => x.DispatchDate)
                .NotNull().WithMessage("Dispatch Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Dispatch Date cannot be in the future.");

            RuleFor(x => x.CarrierName)
                .NotEmpty().WithMessage("Carrier Name is required.")
                .MaximumLength(100).WithMessage("Carrier Name cannot exceed 100 characters.");

            RuleFor(x => x.DispatchDetails)
                .NotNull().WithMessage("Dispatch Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.DispatchDetails)
                .SetValidator(new CreateDispatchDetailValidator(_salesDetailRepository));
        }

        private async Task<bool> BeUniqueDispatchNo(string dispatchNo, CancellationToken cancellationToken)
        {
            return await _dispatchMasterRepository.IsDispatchNoUniqueAsync(dispatchNo, 1, null);
        }
    }

    public class CreateDispatchDetailValidator : AbstractValidator<CreateDispatchDetailRequest>
    {
        private readonly ISalesDetailRepository _salesDetailRepository;

        public CreateDispatchDetailValidator(ISalesDetailRepository salesDetailRepository)
        {
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.SalesDetailId)
                .NotEmpty().WithMessage("Valid Sales Detail is required.")
                .MustAsync(SalesDetailExists).WithMessage("Sales Detail not found.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity must be greater than 0.")
                .Must((request, qty) => qty <= GetSalesDetailRemainingQty(request.SalesDetailId).Result)
                .WithMessage("Quantity cannot exceed remaining sales quantity (after picked/shipped).");
        }

        private async Task<bool> SalesDetailExists(int salesDetailId, CancellationToken cancellationToken)
        {
            var detail = await _salesDetailRepository.GetByIdAsync(salesDetailId);
            return detail != null && detail.IsDeleted != true;
        }

        private async Task<int> GetSalesDetailRemainingQty(int salesDetailId)
        {
            var detail = await _salesDetailRepository.GetByIdAsync(salesDetailId);
            if (detail == null) return 0;
            return detail.Quantity - (detail.ShippedQty ?? 0);
        }
    }
}
