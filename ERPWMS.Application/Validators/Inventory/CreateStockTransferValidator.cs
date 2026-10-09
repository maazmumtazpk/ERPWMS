using ERPWMS.Application.DTOs.Inventory;

using FluentValidation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Inventory
{
    public class CreateStockTransferValidator : AbstractValidator<CreateStockTransferRequest>
    {
        public CreateStockTransferValidator()
        {
            RuleFor(x => x.FromWarehouseId)
                .NotEmpty().WithMessage("Valid From Warehouse is required.");

            RuleFor(x => x.ToWarehouseId)
                .NotEmpty().WithMessage("Valid To Warehouse is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.TransferNo)
                .NotEmpty().WithMessage("Transfer Number is required.")
                .MaximumLength(50).WithMessage("Transfer Number cannot exceed 50 characters.");

            RuleFor(x => x.FromWarehouseId)
                .NotEqual(x => x.ToWarehouseId)
                .WithMessage("From Warehouse and To Warehouse cannot be the same.");

            RuleFor(x => x.TransferDetails)
                .Must(details => details != null && details.Count > 0)
                .WithMessage("At least one transfer detail is required.");

            RuleForEach(x => x.TransferDetails)
                .SetValidator(new CreateStockTransferDetailValidator());
        }
    }

    public class CreateStockTransferDetailValidator : AbstractValidator<CreateStockTransferDetailRequest>
    {
        public CreateStockTransferDetailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Valid Product is required.");

            RuleFor(x => x.FromBinId)
                .NotEmpty().WithMessage("Valid From Bin is required.");

            RuleFor(x => x.ToBinId)
                .NotEmpty().WithMessage("Valid To Bin is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity must be greater than 0.");
        }
    }
}