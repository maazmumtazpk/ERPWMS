using FluentValidation;
using ERPWMS.Application.DTOs.Company;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Company
{
    
    public class SearchCompaniesValidator : AbstractValidator<SearchCompaniesQuery>
    {
        public SearchCompaniesValidator()
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Page must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrEmpty(sortBy) ||
                    new[] { "companyname", "registrationnumber", "createddate", "isactive" }.Contains(sortBy.ToLower()))
                .WithMessage("Invalid sort column. Allowed: companyname, registrationnumber, createddate, isactive.");
        }
    }

    
    public class GetCompanyProfileValidator : AbstractValidator<GetCompanyProfileQuery>
    {
        public GetCompanyProfileValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");
        }
    }

    
    public class GetCompanyByNameValidator : AbstractValidator<GetCompanyByNameQuery>
    {
        public GetCompanyByNameValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");
        }
    }

   
    public class ValidateCompanyNameValidator : AbstractValidator<ValidateCompanyNameRequest>
    {
        public ValidateCompanyNameValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");
        }
    }

    
    public class ValidateRegistrationNumberValidator : AbstractValidator<ValidateRegistrationNumberRequest>
    {
        public ValidateRegistrationNumberValidator()
        {
            RuleFor(x => x.RegistrationNumber)
                .NotEmpty().WithMessage("Registration number is required.")
                .MaximumLength(50).WithMessage("Registration number cannot exceed 50 characters.")
                .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("Registration number contains invalid characters.");
        }
    }
}