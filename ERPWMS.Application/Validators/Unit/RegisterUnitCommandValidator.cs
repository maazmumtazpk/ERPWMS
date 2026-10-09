using FluentValidation;
using ERPWMS.Application.Commands.Unit;

namespace ERPWMS.Application.Validators.Unit
{
    public class RegisterUnitCommandValidator : AbstractValidator<RegisterUnitCommand>
    {
        public RegisterUnitCommandValidator()
        {
            RuleFor(x => x.UnitName)
                .NotEmpty().WithMessage("Unit name is required")
                .MaximumLength(50).WithMessage("Unit name cannot exceed 50 characters");

            RuleFor(x => x.UnitCode)
                .MaximumLength(20).WithMessage("Unit code cannot exceed 20 characters")
                .Matches(@"^[A-Z0-9]*$").WithMessage("Unit code can only contain uppercase letters and numbers")
                .When(x => !string.IsNullOrEmpty(x.UnitCode));

            RuleFor(x => x.IsBaseUnit)
                .NotNull().WithMessage("Base unit indicator is required");

            RuleFor(x => x)
                .Must(x => x.IsBaseUnit == true || (x.BaseUnitId.HasValue && x.BaseUnitId.Value > 0))
                .WithMessage("Non-base units must have a base unit selected")
                .Must(x => x.IsBaseUnit != true || !x.BaseUnitId.HasValue)
                .WithMessage("Base units cannot have a base unit reference")
                .Must(x => x.IsBaseUnit != true || !x.ConversionFactor.HasValue)
                .WithMessage("Base units cannot have a conversion factor");

            RuleFor(x => x.ConversionFactor)
                .NotEmpty().WithMessage("Conversion factor must be greater than 0")
                .When(x => x.ConversionFactor.HasValue && x.IsBaseUnit == false);

            RuleFor(x => x.BaseUnitId)
                .NotEmpty().WithMessage("Invalid base unit selection")
                .When(x => x.BaseUnitId.HasValue && x.IsBaseUnit == false);

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy must be a valid user ID");
        }
    }
}
