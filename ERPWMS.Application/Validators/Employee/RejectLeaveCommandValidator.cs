using ERPWMS.Application.DTOs.Employee;
using FluentValidation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Employee
{
    public class RejectLeaveCommandValidator
        : AbstractValidator<RejectLeaveCommand>
    {
        public RejectLeaveCommandValidator()
        {
            RuleFor(x => x.LeaveId)
                .NotEmpty()
                .WithMessage("Leave ID must be greater than 0.");

            RuleFor(x => x.RejectionReason)
                .NotEmpty()
                .WithMessage("Rejection reason is required.")
                .MaximumLength(500)
                .WithMessage("Rejection reason cannot exceed 500 characters.");
        }
    }
}