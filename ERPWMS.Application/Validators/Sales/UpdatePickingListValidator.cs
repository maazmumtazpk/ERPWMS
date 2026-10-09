
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
    public class UpdatePickingListValidator : AbstractValidator<UpdatePickingListRequest>
    {
        private readonly IPickingListRepository _pickingListRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;

        public UpdatePickingListValidator(
            IPickingListRepository pickingListRepository,
            ISalesDetailRepository salesDetailRepository)
        {
            _pickingListRepository = pickingListRepository;
            _salesDetailRepository = salesDetailRepository;

            RuleFor(x => x.PickingId)
                .NotEmpty()
                .WithMessage("Valid Picking ID is required.");

            RuleFor(x => x.SalesId)
                .NotEmpty()
                .WithMessage("Valid Sales Order is required.");

            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("Valid Warehouse is required.");

            RuleFor(x => x.PickingNo)
                .NotEmpty()
                .WithMessage("Picking Number is required.")
                .MaximumLength(50)
                .WithMessage("Picking Number cannot exceed 50 characters.")
                .MustAsync(BeUniquePickingNo)
                .WithMessage("Picking Number already exists.");

            RuleFor(x => x.PickingDate)
                .NotNull()
                .WithMessage("Picking Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Picking Date cannot be in the future.");

            RuleFor(x => x.Status)
                .Must(status =>
                    string.IsNullOrEmpty(status) ||
                    new[]
                    {
                        "Draft",
                        "Assigned",
                        "Picked",
                        "Verified",
                        "Completed",
                        "Cancelled"
                    }.Contains(status))
                .WithMessage(
                    "Invalid Status. Allowed: Draft, Assigned, Picked, Verified, Completed, Cancelled.");

            RuleFor(x => x.PickingDetails)
                .NotNull()
                .WithMessage("Picking Details are required.")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one item is required.");

            // IMPORTANT:
            // PickingDetails ka type CreatePickingDetailRequest hai.
            // Is liye CreatePickingDetailValidator use hoga.
            RuleForEach(x => x.PickingDetails)
                .SetValidator(
                    new CreatePickingDetailValidator(_salesDetailRepository));
        }

        private async Task<bool> BeUniquePickingNo(
            UpdatePickingListRequest request,
            string pickingNo,
            CancellationToken cancellationToken)
        {
            return await _pickingListRepository.IsPickingNoUniqueAsync(
                pickingNo,
                1,
                request.PickingId);
        }
    }
}


