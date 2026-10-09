using FluentValidation;
using ERPWMS.Application.Commands.Category;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Category
{
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Invalid category ID");

            RuleFor(x => x.DeletedBy)
                .NotEmpty().WithMessage("DeletedBy must be a valid user ID");
        }
    }
}
