using FluentValidation;
using ERPWMS.Application.DTOs.Department;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Department
{
    /// <summary>
    /// Validator for SearchDepartmentsQuery
    /// </summary>
    public class SearchDepartmentsValidator : AbstractValidator<SearchDepartmentsQuery>
    {
        public SearchDepartmentsValidator()
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Page must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrEmpty(sortBy) ||
                    new[] { "departmentname", "departmentcode", "createddate", "isactive" }.Contains(sortBy.ToLower()))
                .WithMessage("Invalid sort column. Allowed: departmentname, departmentcode, createddate, isactive.");
        }
    }

    /// <summary>
    /// Validator for GetDepartmentProfileQuery
    /// </summary>
    public class GetDepartmentProfileValidator : AbstractValidator<GetDepartmentProfileQuery>
    {
        public GetDepartmentProfileValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("A valid department ID is required.");
        }
    }

    /// <summary>
    /// Validator for GetDepartmentByCodeQuery
    /// </summary>
    public class GetDepartmentByCodeValidator : AbstractValidator<GetDepartmentByCodeQuery>
    {
        public GetDepartmentByCodeValidator()
        {
            RuleFor(x => x.DepartmentCode)
                .NotEmpty().WithMessage("Department code is required.")
                .MaximumLength(20).WithMessage("Department code cannot exceed 20 characters.");

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for GetActiveDepartmentsQuery
    /// </summary>
    public class GetActiveDepartmentsValidator : AbstractValidator<GetActiveDepartmentsQuery>
    {
        public GetActiveDepartmentsValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for GetDepartmentsByBranchQuery
    /// </summary>
    public class GetDepartmentsByBranchValidator : AbstractValidator<GetDepartmentsByBranchQuery>
    {
        public GetDepartmentsByBranchValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for GetDepartmentsByCompanyQuery
    /// </summary>
    public class GetDepartmentsByCompanyValidator : AbstractValidator<GetDepartmentsByCompanyQuery>
    {
        public GetDepartmentsByCompanyValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    /// <summary>
    /// Validator for ValidateDepartmentNameRequest
    /// </summary>
    public class ValidateDepartmentNameValidator : AbstractValidator<ValidateDepartmentNameRequest>
    {
        public ValidateDepartmentNameValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(200).WithMessage("Department name cannot exceed 200 characters.");

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for ValidateDepartmentCodeRequest
    /// </summary>
    public class ValidateDepartmentCodeValidator : AbstractValidator<ValidateDepartmentCodeRequest>
    {
        public ValidateDepartmentCodeValidator()
        {
            RuleFor(x => x.DepartmentCode)
                .NotEmpty().WithMessage("Department code is required.")
                .MaximumLength(20).WithMessage("Department code cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("Department code contains invalid characters.");

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for ValidateBranchForDepartmentRequest
    /// </summary>
    public class ValidateBranchForDepartmentValidator : AbstractValidator<ValidateBranchForDepartmentRequest>
    {
        public ValidateBranchForDepartmentValidator()
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("A valid branch ID is required.");
        }
    }

    /// <summary>
    /// Validator for ValidateCompanyForDepartmentRequest
    /// </summary>
    public class ValidateCompanyForDepartmentValidator : AbstractValidator<ValidateCompanyForDepartmentRequest>
    {
        public ValidateCompanyForDepartmentValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }
}

