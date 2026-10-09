using FluentValidation;
using ERPWMS.Application.Commands.Employee.RegisterEmployee;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Employee
{
    public class RegisterEmployeeCommandValidator : AbstractValidator<RegisterEmployeeCommand>
    {
        public RegisterEmployeeCommandValidator()
        {
            RuleFor(x => x.Request.DepartmentId)
                .NotEmpty().WithMessage("Department is required");

            RuleFor(x => x.Request.DesignationId)
                .NotEmpty().WithMessage("Designation is required");

            RuleFor(x => x.Request.BranchId)
                .NotEmpty().WithMessage("Branch is required");

            RuleFor(x => x.Request.EmployeeCode)
                .NotEmpty().WithMessage("Employee code is required")
                .MaximumLength(50).WithMessage("Employee code cannot exceed 50 characters");

            RuleFor(x => x.Request.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.Request.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Request.FatherName)
                .MaximumLength(100).WithMessage("Father name cannot exceed 100 characters");

            RuleFor(x => x.Request.Cnic)
                .MaximumLength(20).WithMessage("CNIC cannot exceed 20 characters");

            RuleFor(x => x.Request.Gender)
                .Must(g => string.IsNullOrEmpty(g) || g == "Male" || g == "Female")
                .WithMessage("Gender must be Male or Female");

            RuleFor(x => x.Request.HireDate)
                .NotEmpty().WithMessage("Hire date is required");

            RuleFor(x => x.Request.BaseSalary)
                .GreaterThanOrEqualTo(0).WithMessage("Base salary must be greater than or equal to 0");
        }
    }
}
