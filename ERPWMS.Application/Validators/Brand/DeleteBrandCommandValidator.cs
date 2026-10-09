using FluentValidation;
using ERPWMS.Application.Commands.Brand;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Brand
{
    public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
    {
        public DeleteBrandCommandValidator()
        {
            RuleFor(x => x.BrandId)
                .NotEmpty().WithMessage("Invalid brand ID");

            RuleFor(x => x.DeletedBy)
                .NotEmpty().WithMessage("DeletedBy must be a valid user ID");
        }
    }
}
