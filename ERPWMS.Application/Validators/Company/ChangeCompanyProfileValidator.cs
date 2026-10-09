using FluentValidation;
using ERPWMS.Application.DTOs.Company;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Company
{
    /// <summary>
    /// Validator for ChangeCompanyProfileRequest
    /// </summary>
    public class ChangeCompanyProfileValidator : AbstractValidator<ChangeCompanyProfileRequest>
    {
        public ChangeCompanyProfileValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("A valid company ID is required.");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.")
                .MinimumLength(2).WithMessage("Company name must be at least 2 characters.");

            RuleFor(x => x.RegistrationNumber)
                .MaximumLength(50).WithMessage("Registration number cannot exceed 50 characters.")
                .Matches(@"^[a-zA-Z0-9\-_]*$").WithMessage("Registration number contains invalid characters.")
                .When(x => !string.IsNullOrEmpty(x.RegistrationNumber));

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

            RuleFor(x => x.LogoUrl)
                .MaximumLength(500).WithMessage("Logo URL cannot exceed 500 characters.")
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Logo URL must be a valid URL.")
                .When(x => !string.IsNullOrEmpty(x.LogoUrl));
        }
    }
}