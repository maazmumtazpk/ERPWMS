using FluentValidation;
using ERPWMS.Application.DTOs.Payroll;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Payroll
{
    public class GeneratePayrollValidator
        : AbstractValidator<GeneratePayrollRequest>
    {
        public GeneratePayrollValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee is required.");

            RuleFor(x => x.MonthYear)
                .NotEmpty()
                .WithMessage("Payroll month is required.");
        }
    }


    public class ApprovePayrollValidator
        : AbstractValidator<ApprovePayrollRequest>
    {
        public ApprovePayrollValidator()
        {
            RuleFor(x => x.PayrollId)
                .NotEmpty()
                .WithMessage("Payroll is required.");
        }
    }


    public class MarkPayrollPaidValidator
        : AbstractValidator<MarkPayrollPaidRequest>
    {
        public MarkPayrollPaidValidator()
        {
            RuleFor(x => x.PayrollId)
                .NotEmpty();

            RuleFor(x => x.PaymentDate)
                .NotEmpty();
        }
    }
}