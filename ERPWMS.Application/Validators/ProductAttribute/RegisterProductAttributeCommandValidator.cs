using FluentValidation;
using ERPWMS.Application.Commands.ProductAttribute;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.ProductAttribute
{
    public class RegisterProductAttributeCommandValidator : AbstractValidator<RegisterProductAttributeCommand>
    {
        public RegisterProductAttributeCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product selection is required");

            RuleFor(x => x.AttributeName)
                .NotEmpty().WithMessage("Attribute name is required")
                .MaximumLength(100).WithMessage("Attribute name cannot exceed 100 characters");

            RuleFor(x => x.AttributeValue)
                .NotEmpty().WithMessage("Attribute value is required")
                .MaximumLength(500).WithMessage("Attribute value cannot exceed 500 characters");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy must be a valid user ID");
        }
    }
}
