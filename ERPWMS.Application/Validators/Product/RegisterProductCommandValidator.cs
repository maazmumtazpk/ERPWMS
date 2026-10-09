using FluentValidation;
using ERPWMS.Application.Commands.Product;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Product
{
    public class RegisterProductCommandValidator : AbstractValidator<RegisterProductCommand>
    {
        public RegisterProductCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category selection is required");

            RuleFor(x => x.SubCategoryId)
                .NotEmpty().WithMessage("SubCategory selection is required");

            RuleFor(x => x.BrandId)
                .NotEmpty().WithMessage("Brand selection is required");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit selection is required");

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required")
                .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters")
                .Matches(@"^[A-Z0-9\-_]*$").WithMessage("SKU can only contain uppercase letters, numbers, hyphens, and underscores");

            RuleFor(x => x.Barcode)
                .MaximumLength(100).WithMessage("Barcode cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Barcode));

            RuleFor(x => x.QRCode)
                .MaximumLength(200).WithMessage("QRCode cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.QRCode));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => x.Description != null);

            RuleFor(x => x.Weight)
                .NotEmpty().WithMessage("Weight must be greater than 0")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Length)
                .NotEmpty().WithMessage("Length must be greater than 0")
                .When(x => x.Length.HasValue);

            RuleFor(x => x.Width)
                .NotEmpty().WithMessage("Width must be greater than 0")
                .When(x => x.Width.HasValue);

            RuleFor(x => x.Height)
                .NotEmpty().WithMessage("Height must be greater than 0")
                .When(x => x.Height.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinStockLevel.HasValue || !x.MaxStockLevel.HasValue || x.MinStockLevel <= x.MaxStockLevel)
                .WithMessage("MinStockLevel cannot be greater than MaxStockLevel");

            RuleFor(x => x)
                .Must(x => !x.ReorderLevel.HasValue || !x.MaxStockLevel.HasValue || x.ReorderLevel <= x.MaxStockLevel)
                .WithMessage("ReorderLevel cannot be greater than MaxStockLevel");

            RuleFor(x => x)
                .Must(x => !x.MinStockLevel.HasValue || !x.ReorderLevel.HasValue || x.MinStockLevel <= x.ReorderLevel)
                .WithMessage("MinStockLevel cannot be greater than ReorderLevel");

            RuleFor(x => x.MinStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("MinStockLevel must be 0 or greater")
                .When(x => x.MinStockLevel.HasValue);

            RuleFor(x => x.MaxStockLevel)
                .NotEmpty().WithMessage("MaxStockLevel must be greater than 0")
                .When(x => x.MaxStockLevel.HasValue);

            RuleFor(x => x.ReorderLevel)
                .GreaterThanOrEqualTo(0).WithMessage("ReorderLevel must be 0 or greater")
                .When(x => x.ReorderLevel.HasValue);

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy must be a valid user ID");
        }
    }
}
