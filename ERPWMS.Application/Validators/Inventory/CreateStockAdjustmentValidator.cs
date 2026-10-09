using ERPWMS.Application.DTOs.Inventory;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Inventory
{
    public class CreateStockAdjustmentValidator : AbstractValidator<CreateStockAdjustmentRequest>
    {
        private readonly IStockAdjustmentRepository _adjustmentRepository;

        public CreateStockAdjustmentValidator(IStockAdjustmentRepository adjustmentRepository)
        {
            _adjustmentRepository = adjustmentRepository;

            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Valid Warehouse is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.BinId)
                .NotEmpty().WithMessage("Valid Bin is required.");

            RuleFor(x => x.AdjustmentType)
                .NotEmpty().WithMessage("Adjustment Type is required.")
                .MaximumLength(20).WithMessage("Adjustment Type cannot exceed 20 characters.")
                .Must(type => type == "Increase" || type == "Decrease")
                .WithMessage("Adjustment Type must be 'Increase' or 'Decrease'.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");

            RuleFor(x => x.AdjustmentDetails)
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one adjustment detail is required.");

            RuleForEach(x => x.AdjustmentDetails)
                .SetValidator(new CreateStockAdjustmentDetailValidator());
        }
    }

    public class CreateStockAdjustmentDetailValidator : AbstractValidator<CreateStockAdjustmentDetailRequest>
    {
        public CreateStockAdjustmentDetailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.BinId)
                .NotEmpty().WithMessage("Valid Bin is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity must be greater than 0.");
        }
    }
}
