using FluentValidation;
using ERPWMS.Application.DTOs.Attendance;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Attendance
{
    public class CheckInAttendanceValidator
        : AbstractValidator<CheckInAttendanceRequest>
    {
        public CheckInAttendanceValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee is required.");

            RuleFor(x => x.AttendanceDate)
                .NotEmpty()
                .WithMessage("Attendance date is required.");

            RuleFor(x => x.CheckInTime)
                .NotEmpty()
                .WithMessage("Check-in time is required.");
        }
    }

    public class CheckOutAttendanceValidator
        : AbstractValidator<CheckOutAttendanceRequest>
    {
        public CheckOutAttendanceValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee is required.");

            RuleFor(x => x.AttendanceDate)
                .NotEmpty()
                .WithMessage("Attendance date is required.");

            RuleFor(x => x.CheckOutTime)
                .NotEmpty()
                .WithMessage("Check-out time is required.");
        }
    }

    public class MarkAbsentValidator
        : AbstractValidator<MarkAbsentRequest>
    {
        public MarkAbsentValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee is required.");

            RuleFor(x => x.AttendanceDate)
                .NotEmpty()
                .WithMessage("Attendance date is required.");
        }
    }

    public class MarkLateValidator
        : AbstractValidator<MarkLateRequest>
    {
        public MarkLateValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee is required.");

            RuleFor(x => x.AttendanceDate)
                .NotEmpty()
                .WithMessage("Attendance date is required.");

            RuleFor(x => x.CheckInTime)
                .NotEmpty()
                .WithMessage("Check-in time is required.");
        }
    }
}
