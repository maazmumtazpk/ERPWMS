using FluentValidation;
using ERPWMS.Application.DTOs.Department;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Department
{
    /// <summary>
    /// Validator for ChangeDepartmentProfileRequest
    /// </summary>
    public class ChangeDepartmentProfileValidator : AbstractValidator<ChangeDepartmentProfileRequest>
    {
        public ChangeDepartmentProfileValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");

            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(200).WithMessage("Department name cannot exceed 200 characters.")
                .MinimumLength(2).WithMessage("Department name must be at least 2 characters.");

            RuleFor(x => x.DepartmentCode)
                .MaximumLength(20).WithMessage("Department code cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9\-_]*$").WithMessage("Department code contains invalid characters.")
                .When(x => !string.IsNullOrEmpty(x.DepartmentCode));
        }
    }
}

