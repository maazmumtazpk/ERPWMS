using ERPWMS.Application.DTOs.Employee;
using FluentValidation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Employee
{
    public class DeleteEmployeeCommandValidator
        : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee ID must be greater than 0.");
        }
    }
}