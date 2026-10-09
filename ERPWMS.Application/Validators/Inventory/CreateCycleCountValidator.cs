using ERPWMS.Application.DTOs.Inventory;
using FluentValidation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Inventory
{
    public class CreateCycleCountValidator : AbstractValidator<CreateCycleCountRequest>
    {
        public CreateCycleCountValidator()
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Valid Warehouse is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.BinId)
                .NotEmpty().WithMessage("Valid Bin is required.");

            RuleFor(x => x.CycleCountDetails)
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one cycle count detail is required.");

            RuleForEach(x => x.CycleCountDetails)
                .SetValidator(new CreateCycleCountDetailValidator());
        }
    }

    public class CreateCycleCountDetailValidator : AbstractValidator<CreateCycleCountDetailRequest>
    {
        public CreateCycleCountDetailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.BinId)
                .NotEmpty().WithMessage("Valid Bin is required.");

            RuleFor(x => x.SystemQty)
                .GreaterThanOrEqualTo(0).WithMessage("System Quantity cannot be negative.");

            RuleFor(x => x.ActualQty)
                .GreaterThanOrEqualTo(0).WithMessage("Actual Quantity cannot be negative.");
        }
    }
}