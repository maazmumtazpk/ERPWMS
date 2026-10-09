using FluentValidation;
using ERPWMS.Application.DTOs.Branch;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Branch
{
    /// <summary>
    /// Validator for ActivateBranchCommand
    /// </summary>
    public class ActivateBranchValidator : AbstractValidator<ActivateBranchCommand>
    {
        public ActivateBranchValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for DeactivateBranchCommand
    /// </summary>
    public class DeactivateBranchValidator : AbstractValidator<DeactivateBranchCommand>
    {
        public DeactivateBranchValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Reason));
        }
    }
}