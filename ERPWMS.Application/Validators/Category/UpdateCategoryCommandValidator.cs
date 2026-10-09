using FluentValidation;
using ERPWMS.Application.Commands.Category;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Category
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Invalid category ID");

            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

            RuleFor(x => x.CategoryCode)
                .MaximumLength(50).WithMessage("Category code cannot exceed 50 characters")
                .Matches(@"^[A-Z0-9-]*$").WithMessage("Category code can only contain uppercase letters, numbers, and hyphens")
                .When(x => !string.IsNullOrEmpty(x.CategoryCode));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => x.Description != null);
        }
    }
}
