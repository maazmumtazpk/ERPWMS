using ERPWMS.Application.DTOs.Leave;
using FluentValidation;
using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Leave
{
    public class SubmitLeaveValidator
        : AbstractValidator<SubmitLeaveRequest>
    {
        public SubmitLeaveValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee is required.");

            RuleFor(x => x.LeaveType)
                .NotEmpty()
                .WithMessage("Leave type is required.")
                .Must(IsValidLeaveType)
                .WithMessage(
                    "Leave type must be Annual, Sick, Casual, Maternity, Paternity or Unpaid.");

            RuleFor(x => x.FromDate)
                .NotEmpty()
                .WithMessage("From date is required.");

            RuleFor(x => x.ToDate)
                .NotEmpty()
                .WithMessage("To date is required.")
                .GreaterThanOrEqualTo(x => x.FromDate)
                .WithMessage(
                    "To date cannot be earlier than From date.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Leave reason is required.")
                .MaximumLength(500)
                .WithMessage(
                    "Reason cannot exceed 500 characters.");
        }

        private static bool IsValidLeaveType(
            string leaveType)
        {
            return leaveType.Equals(
                       "Annual",
                       StringComparison.OrdinalIgnoreCase)
                   || leaveType.Equals(
                       "Sick",
                       StringComparison.OrdinalIgnoreCase)
                   || leaveType.Equals(
                       "Casual",
                       StringComparison.OrdinalIgnoreCase)
                   || leaveType.Equals(
                       "Maternity",
                       StringComparison.OrdinalIgnoreCase)
                   || leaveType.Equals(
                       "Paternity",
                       StringComparison.OrdinalIgnoreCase)
                   || leaveType.Equals(
                       "Unpaid",
                       StringComparison.OrdinalIgnoreCase);
        }
    }


    public class RejectLeaveValidator
        : AbstractValidator<RejectLeaveRequest>
    {
        public RejectLeaveValidator()
        {
            RuleFor(x => x.LeaveId)
                .NotEmpty()
                .WithMessage("Leave ID is required.");

            RuleFor(x => x.RejectionReason)
                .NotEmpty()
                .WithMessage("Rejection reason is required.")
                .MaximumLength(500)
                .WithMessage(
                    "Rejection reason cannot exceed 500 characters.");
        }
    }
}