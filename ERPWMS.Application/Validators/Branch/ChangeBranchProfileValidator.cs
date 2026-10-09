using FluentValidation;
using ERPWMS.Application.DTOs.Branch;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Branch
{
    /// <summary>
    /// Validator for ChangeBranchProfileRequest
    /// </summary>
    public class ChangeBranchProfileValidator : AbstractValidator<ChangeBranchProfileRequest>
    {
        public ChangeBranchProfileValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");

            RuleFor(x => x.BranchName)
                .NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters.")
                .MinimumLength(2).WithMessage("Branch name must be at least 2 characters.");

            RuleFor(x => x.BranchCode)
                .MaximumLength(20).WithMessage("Branch code cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9\-_]*$").WithMessage("Branch code contains invalid characters.")
                .When(x => !string.IsNullOrEmpty(x.BranchCode));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email address format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }
}
