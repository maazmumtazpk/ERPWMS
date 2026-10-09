using FluentValidation;
using ERPWMS.Application.DTOs.Designation;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Designation
{
    public class SearchDesignationsValidator : AbstractValidator<SearchDesignationsQuery>
    {
        public SearchDesignationsValidator()
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Page must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrEmpty(sortBy) ||
                    new[] { "designationname", "ranklevel", "createddate", "isactive" }.Contains(sortBy.ToLower()))
                .WithMessage("Invalid sort column. Allowed: designationname, ranklevel, createddate, isactive.");
        }
    }

    public class GetDesignationProfileValidator : AbstractValidator<GetDesignationProfileQuery>
    {
        public GetDesignationProfileValidator()
        {
            RuleFor(x => x.DesignationId)
                .NotEmpty().WithMessage("A valid designation ID is required.");
        }
    }

    public class GetActiveDesignationsValidator : AbstractValidator<GetActiveDesignationsQuery>
    {
        public GetActiveDesignationsValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");
        }
    }

    public class GetDesignationsByDepartmentValidator : AbstractValidator<GetDesignationsByDepartmentQuery>
    {
        public GetDesignationsByDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");
        }
    }

    public class GetDesignationsByCompanyValidator : AbstractValidator<GetDesignationsByCompanyQuery>
    {
        public GetDesignationsByCompanyValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    public class ValidateDesignationNameValidator : AbstractValidator<ValidateDesignationNameRequest>
    {
        public ValidateDesignationNameValidator()
        {
            RuleFor(x => x.DesignationName)
                .NotEmpty().WithMessage("Designation name is required.")
                .MaximumLength(200).WithMessage("Designation name cannot exceed 200 characters.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");
        }
    }

    public class ValidateDepartmentForDesignationValidator : AbstractValidator<ValidateDepartmentForDesignationRequest>
    {
        public ValidateDepartmentForDesignationValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");
        }
    }

    public class ValidateCompanyForDesignationValidator : AbstractValidator<ValidateCompanyForDesignationRequest>
    {
        public ValidateCompanyForDesignationValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }
}

