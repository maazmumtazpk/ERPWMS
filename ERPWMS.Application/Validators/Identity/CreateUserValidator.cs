using ERPWMS.Application.DTOs.Identity;
using ERPWMS.Domain.Services;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Validators.Identity
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        private readonly IUserService _userService;

        public CreateUserValidator(IUserService userService)
        {
            _userService = userService;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
                .MustAsync(BeUniqueUsername).WithMessage("Username already exists.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MustAsync(BeUniqueEmail).WithMessage("Email already exists.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }

        private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
        {
            return !await _userService.ExistsByUsernameAsync(username);
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return !await _userService.ExistsByEmailAsync(email);
        }
    }
}