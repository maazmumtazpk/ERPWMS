using FluentValidation;
using ERPWMS.Application.DTOs.Designation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Designation
{
    public class RegisterDesignationValidator : AbstractValidator<RegisterDesignationRequest>
    {
        public RegisterDesignationValidator()
        {
            RuleFor(x => x.DesignationName)
                .NotEmpty().WithMessage("Designation name is required.")
                .MaximumLength(200).WithMessage("Designation name cannot exceed 200 characters.")
                .MinimumLength(2).WithMessage("Designation name must be at least 2 characters.");

            RuleFor(x => x.RankLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Rank level cannot be negative.")
                .When(x => x.RankLevel.HasValue);

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");
        }
    }
}
