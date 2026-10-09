using FluentValidation;
using ERPWMS.Application.Commands.SubCategory;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.SubCategory
{
    public class RegisterSubCategoryCommandValidator : AbstractValidator<RegisterSubCategoryCommand>
    {
        public RegisterSubCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category selection is required");

            RuleFor(x => x.SubCategoryName)
                .NotEmpty().WithMessage("SubCategory name is required")
                .MaximumLength(100).WithMessage("SubCategory name cannot exceed 100 characters");

            RuleFor(x => x.SubCategoryCode)
                .MaximumLength(50).WithMessage("SubCategory code cannot exceed 50 characters")
                .Matches(@"^[A-Z0-9-]*$").WithMessage("SubCategory code can only contain uppercase letters, numbers, and hyphens")
                .When(x => !string.IsNullOrEmpty(x.SubCategoryCode));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => x.Description != null);

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy must be a valid user ID");
        }
    }
}
