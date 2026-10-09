using FluentValidation;
using ERPWMS.Application.Commands.Shift.Create_Shift;

namespace ERPWMS.Application.Validators.Shift
{
    public class CreateShiftCommandValidator
        : AbstractValidator<CreateShiftCommand>
    {
        public CreateShiftCommandValidator()
        {
            // =====================================================
            // SHIFT NAME
            // =====================================================

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("Shift request is required.");

            RuleFor(x => x.Request.ShiftName)
                .NotEmpty()
                .WithMessage("Shift name is required.")
                .MaximumLength(50)
                .WithMessage("Shift name cannot exceed 50 characters.");

            // =====================================================
            // START TIME
            // =====================================================

            RuleFor(x => x.Request.StartTime)
                .NotNull()
                .WithMessage("Start time is required.");

            // =====================================================
            // END TIME
            // =====================================================

            RuleFor(x => x.Request.EndTime)
                .NotNull()
                .WithMessage("End time is required.");

            // =====================================================
            // TIME ORDER
            // =====================================================

            RuleFor(x => x.Request)
                .Must(x =>
                    x.StartTime.HasValue &&
                    x.EndTime.HasValue &&
                    x.StartTime.Value < x.EndTime.Value)
                .WithMessage("Start time must be before end time.")
                .When(x =>
                    x.Request != null &&
                    x.Request.StartTime.HasValue &&
                    x.Request.EndTime.HasValue);

            // =====================================================
            // GRACE PERIOD
            // =====================================================

            RuleFor(x => x.Request.GracePeriodMinutes)
                .GreaterThanOrEqualTo(0)
                .WithMessage(
                    "Grace period must be greater than or equal to 0.")
                .When(x =>
                    x.Request != null &&
                    x.Request.GracePeriodMinutes.HasValue);
        }
    }
}