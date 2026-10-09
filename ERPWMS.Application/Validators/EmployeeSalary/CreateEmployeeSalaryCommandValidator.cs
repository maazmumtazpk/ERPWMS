using FluentValidation;
using ERPWMS.Application.Commands.EmployeeSalary.CreateEmployeeSalary;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.EmployeeSalary
{
    public class CreateEmployeeSalaryCommandValidator : AbstractValidator<CreateEmployeeSalaryCommand>
    {
        public CreateEmployeeSalaryCommandValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .NotEmpty().WithMessage("Employee is required");

            RuleFor(x => x.Request.EffectiveDate)
                .NotEmpty().WithMessage("Effective date is required");

            RuleFor(x => x.Request.BasicSalary)
                .NotEmpty().WithMessage("Basic salary must be greater than 0");

            RuleFor(x => x.Request.HouseRent)
                .GreaterThanOrEqualTo(0).WithMessage("House rent must be greater than or equal to 0")
                .When(x => x.Request.HouseRent.HasValue);

            RuleFor(x => x.Request.UtilityAllowance)
                .GreaterThanOrEqualTo(0).WithMessage("Utility allowance must be greater than or equal to 0")
                .When(x => x.Request.UtilityAllowance.HasValue);

            RuleFor(x => x.Request.MedicalAllowance)
                .GreaterThanOrEqualTo(0).WithMessage("Medical allowance must be greater than or equal to 0")
                .When(x => x.Request.MedicalAllowance.HasValue);

            RuleFor(x => x.Request.OtherAllowances)
                .GreaterThanOrEqualTo(0).WithMessage("Other allowances must be greater than or equal to 0")
                .When(x => x.Request.OtherAllowances.HasValue);

            RuleFor(x => x.Request.IncomeTax)
                .GreaterThanOrEqualTo(0).WithMessage("Income tax must be greater than or equal to 0")
                .When(x => x.Request.IncomeTax.HasValue);

            RuleFor(x => x.Request.ProfessionalTax)
                .GreaterThanOrEqualTo(0).WithMessage("Professional tax must be greater than or equal to 0")
                .When(x => x.Request.ProfessionalTax.HasValue);

            RuleFor(x => x.Request.OtherDeductions)
                .GreaterThanOrEqualTo(0).WithMessage("Other deductions must be greater than or equal to 0")
                .When(x => x.Request.OtherDeductions.HasValue);
        }
    }
}
