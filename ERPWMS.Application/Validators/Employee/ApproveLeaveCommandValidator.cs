using ERPWMS.Application.DTOs.Employee;
using FluentValidation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Employee
{
    public class ApproveLeaveCommandValidator
        : AbstractValidator<ApproveLeaveCommand>
    {
        public ApproveLeaveCommandValidator()
        {
            RuleFor(x => x.LeaveId)
                .NotEmpty()
                .WithMessage("Leave ID must be greater than 0.");
        }
    }
}