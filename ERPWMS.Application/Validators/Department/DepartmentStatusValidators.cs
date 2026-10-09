using FluentValidation;
using ERPWMS.Application.DTOs.Department;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Department
{
    /// <summary>
    /// Validator for ActivateDepartmentCommand
    /// </summary>
    public class ActivateDepartmentValidator : AbstractValidator<ActivateDepartmentCommand>
    {
        public ActivateDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");
        }
    }

    /// <summary>
    /// Validator for DeactivateDepartmentCommand
    /// </summary>
    public class DeactivateDepartmentValidator : AbstractValidator<DeactivateDepartmentCommand>
    {
        public DeactivateDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Reason));
        }
    }
}

