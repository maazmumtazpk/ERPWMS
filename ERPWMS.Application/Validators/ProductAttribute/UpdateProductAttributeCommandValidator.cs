using FluentValidation;
using ERPWMS.Application.Commands.ProductAttribute;

namespace ERPWMS.Application.Validators.ProductAttribute
{
    public class UpdateProductAttributeCommandValidator : AbstractValidator<UpdateProductAttributeCommand>
    {
        public UpdateProductAttributeCommandValidator()
        {
            RuleFor(x => x.AttributeId)
                .NotEmpty().WithMessage("Invalid attribute ID");

            RuleFor(x => x.AttributeName)
                .NotEmpty().WithMessage("Attribute name is required")
                .MaximumLength(100).WithMessage("Attribute name cannot exceed 100 characters");

            RuleFor(x => x.AttributeValue)
                .NotEmpty().WithMessage("Attribute value is required")
                .MaximumLength(500).WithMessage("Attribute value cannot exceed 500 characters");
        }
    }
}
