using FluentValidation;
using ERPWMS.Application.Commands.ProductImage;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.ProductImage
{
    public class RegisterProductImageCommandValidator : AbstractValidator<RegisterProductImageCommand>
    {
        public RegisterProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product selection is required");

            RuleFor(x => x.ImageURL)
                .NotEmpty().WithMessage("Image URL is required")
                .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Image URL must be a valid URL");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater")
                .When(x => x.DisplayOrder.HasValue);

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy must be a valid user ID");
        }
    }
}
