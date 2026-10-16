using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Sales
{
    public class CreatePickingListValidator : AbstractValidator<CreatePickingListRequest>
    {
        private readonly IPickingListRepository _pickingListRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;

        public CreatePickingListValidator(
            IPickingListRepository pickingListRepository,
            ISalesDetailRepository salesDetailRepository)
        {
            _pickingListRepository = pickingListRepository;
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.SalesId)
                .NotEmpty().WithMessage("Valid Sales Order is required.");

            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Valid Warehouse is required.");

            RuleFor(x => x.PickingNo)
                .NotEmpty().WithMessage("Picking Number is required.")
                .MaximumLength(50).WithMessage("Picking Number cannot exceed 50 characters.")
                .MustAsync(BeUniquePickingNo).WithMessage("Picking Number already exists.");

            RuleFor(x => x.PickingDate)
                .NotNull().WithMessage("Picking Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Picking Date cannot be in the future.");

            RuleFor(x => x.PickingDetails)
                .NotNull().WithMessage("Picking Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.PickingDetails)
                .SetValidator(new CreatePickingDetailValidator(_salesDetailRepository));
        }

        private async Task<bool> BeUniquePickingNo(string pickingNo, CancellationToken cancellationToken)
        {
            return await _pickingListRepository.IsPickingNoUniqueAsync(pickingNo, 1, null);
        }
    }

    public class CreatePickingDetailValidator : AbstractValidator<CreatePickingDetailRequest>
    {
        private readonly ISalesDetailRepository _salesDetailRepository;

        public CreatePickingDetailValidator(ISalesDetailRepository salesDetailRepository)
        {
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.SalesDetailId)
                .NotEmpty().WithMessage("Valid Sales Detail is required.")
                .MustAsync(SalesDetailExists).WithMessage("Sales Detail not found.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.BinId)
                .NotEmpty().WithMessage("Valid Bin is required.");

            RuleFor(x => x.PickedQty)
                .GreaterThanOrEqualTo(0).WithMessage("Picked Quantity cannot be negative.")
                .Must((request, qty) => qty <= GetSalesDetailQuantity(request.SalesDetailId).Result)
                .WithMessage("Picked Quantity cannot exceed ordered quantity.");
        }

        private async Task<bool> SalesDetailExists(int salesDetailId, CancellationToken cancellationToken)
        {
            var detail = await _salesDetailRepository.GetByIdAsync(salesDetailId);
            return detail != null && detail.IsDeleted != true;
        }

        private async Task<int> GetSalesDetailQuantity(int salesDetailId)
        {
            var detail = await _salesDetailRepository.GetByIdAsync(salesDetailId);
            return detail?.Quantity ?? 0;
        }
    }
}

