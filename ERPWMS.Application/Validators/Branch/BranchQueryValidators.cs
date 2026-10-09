using FluentValidation;
using ERPWMS.Application.DTOs.Branch;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Branch
{
    /// <summary>
    /// Validator for SearchBranchesQuery
    /// </summary>
    public class SearchBranchesValidator : AbstractValidator<SearchBranchesQuery>
    {
        public SearchBranchesValidator()
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Page must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrEmpty(sortBy) ||
                    new[] { "branchname", "branchcode", "createddate", "isactive" }.Contains(sortBy.ToLower()))
                .WithMessage("Invalid sort column. Allowed: branchname, branchcode, createddate, isactive.");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.")
                .When(x => x.CompanyId.HasValue);
        }
    }

    /// <summary>
    /// Validator for GetBranchProfileQuery
    /// </summary>
    public class GetBranchProfileValidator : AbstractValidator<GetBranchProfileQuery>
    {
        public GetBranchProfileValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for GetBranchByCodeQuery
    /// </summary>
    public class GetBranchByCodeValidator : AbstractValidator<GetBranchByCodeQuery>
    {
        public GetBranchByCodeValidator()
        {
            RuleFor(x => x.BranchCode)
                .NotEmpty().WithMessage("Branch code is required.")
                .MaximumLength(20).WithMessage("Branch code cannot exceed 20 characters.");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    /// <summary>
    /// Validator for GetActiveBranchesQuery
    /// </summary>
    public class GetActiveBranchesValidator : AbstractValidator<GetActiveBranchesQuery>
    {
        public GetActiveBranchesValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    /// <summary>
    /// Validator for GetBranchesByCompanyQuery
    /// </summary>
    public class GetBranchesByCompanyValidator : AbstractValidator<GetBranchesByCompanyQuery>
    {
        public GetBranchesByCompanyValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    /// <summary>
    /// Validator for ValidateBranchNameRequest
    /// </summary>
    public class ValidateBranchNameValidator : AbstractValidator<ValidateBranchNameRequest>
    {
        public ValidateBranchNameValidator()
        {
            RuleFor(x => x.BranchName)
                .NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters.");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    /// <summary>
    /// Validator for ValidateBranchCodeRequest
    /// </summary>
    public class ValidateBranchCodeValidator : AbstractValidator<ValidateBranchCodeRequest>
    {
        public ValidateBranchCodeValidator()
        {
            RuleFor(x => x.BranchCode)
                .NotEmpty().WithMessage("Branch code is required.")
                .MaximumLength(20).WithMessage("Branch code cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("Branch code contains invalid characters.");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    /// <summary>
    /// Validator for ValidateCompanyForBranchRequest
    /// </summary>
    public class ValidateCompanyForBranchValidator : AbstractValidator<ValidateCompanyForBranchRequest>
    {
        public ValidateCompanyForBranchValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }
}