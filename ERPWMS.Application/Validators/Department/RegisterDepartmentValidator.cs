using FluentValidation;
using ERPWMS.Application.DTOs.Department;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Department
{
    /// <summary>
    /// Validator for RegisterDepartmentRequest
    /// </summary>
    public class RegisterDepartmentValidator
        : AbstractValidator<RegisterDepartmentRequest>
    {
        public RegisterDepartmentValidator()
        {
            // ============================================================
            // DEPARTMENT NAME
            // ============================================================

            RuleFor(x => x.DepartmentName)
                .NotEmpty()
                .WithMessage("Department name is required.")
                .MaximumLength(200)
                .WithMessage("Department name cannot exceed 200 characters.")
                .MinimumLength(2)
                .WithMessage("Department name must be at least 2 characters.");


            // ============================================================
            // DEPARTMENT CODE
            // ============================================================

            RuleFor(x => x.DepartmentCode)
                .MaximumLength(20)
                .WithMessage("Department code cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9\-_]*$")
                .WithMessage("Department code contains invalid characters.")
                .When(x => !string.IsNullOrEmpty(x.DepartmentCode));


            // ============================================================
            // BRANCH
            // ============================================================

            RuleFor(x => x.BranchId)
                .NotEmpty()
                .WithMessage("A valid branch ID is required.");


           
        }
    }
}