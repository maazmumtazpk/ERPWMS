using ERPWMS.Application.DTOs.Identity;
using FluentValidation;

namespace ERPWMS.Application.Validators.Identity
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage("Username or Email is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}