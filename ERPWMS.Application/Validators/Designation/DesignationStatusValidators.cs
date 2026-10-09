using FluentValidation;
using ERPWMS.Application.DTOs.Designation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Designation
{
    public class ActivateDesignationValidator : AbstractValidator<ActivateDesignationCommand>
    {
        public ActivateDesignationValidator()
        {
            RuleFor(x => x.DesignationId)
                .NotEmpty().WithMessage("A valid designation ID is required.");
        }
    }

    public class DeactivateDesignationValidator : AbstractValidator<DeactivateDesignationCommand>
    {
        public DeactivateDesignationValidator()
        {
            RuleFor(x => x.DesignationId)
                .NotEmpty().WithMessage("A valid designation ID is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Reason));
        }
    }
}


