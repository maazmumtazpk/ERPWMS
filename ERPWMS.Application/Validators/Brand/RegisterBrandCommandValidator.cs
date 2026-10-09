using ERPWMS.Application.Commands.Brand;
using ERPWMS.Application.Validators.Brand;
using ERPWMS.Database.Models;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace ERPWMS.Application.Validators.Brand
{
    public class RegisterBrandCommandValidator : AbstractValidator<RegisterBrandCommand>
    {
        public RegisterBrandCommandValidator()
        {
            RuleFor(x => x.BrandName)
                .NotEmpty().WithMessage("Brand name is required")
                .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters");

            RuleFor(x => x.BrandCode)
                .MaximumLength(50).WithMessage("Brand code cannot exceed 50 characters")
                .Matches(@"^[A-Z0-9-]*$").WithMessage("Brand code can only contain uppercase letters, numbers, and hyphens")
                .When(x => !string.IsNullOrEmpty(x.BrandCode));

            RuleFor(x => x.LogoURL)
                .MaximumLength(500).WithMessage("Logo URL cannot exceed 500 characters")
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Logo URL must be a valid URL")
                .When(x => !string.IsNullOrEmpty(x.LogoURL));

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy must be a valid user ID");
        }
    }
}

