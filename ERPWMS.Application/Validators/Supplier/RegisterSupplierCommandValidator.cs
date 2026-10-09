using FluentValidation;
using ERPWMS.Application.Commands.Supplier.RegisterSupplier;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Supplier
{
    public class RegisterSupplierCommandValidator : AbstractValidator<RegisterSupplierCommand>
    {
        public RegisterSupplierCommandValidator()
        {
            RuleFor(x => x.Request.SupplierName)
                .NotEmpty().WithMessage("Supplier name is required")
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters");

            RuleFor(x => x.Request.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters");

            RuleFor(x => x.Request.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

            RuleFor(x => x.Request.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrWhiteSpace(x.Request.Email));

            RuleFor(x => x.Request.Mobile)
                .MaximumLength(20).WithMessage("Mobile cannot exceed 20 characters");

            RuleFor(x => x.Request.City)
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters");

            RuleFor(x => x.Request.State)
                .MaximumLength(50).WithMessage("State cannot exceed 50 characters");

            RuleFor(x => x.Request.Country)
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters");

            RuleFor(x => x.Request.PostalCode)
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");

            RuleFor(x => x.Request.TaxId)
                .MaximumLength(50).WithMessage("Tax ID cannot exceed 50 characters");

            RuleFor(x => x.Request.NTN)
                .MaximumLength(50).WithMessage("NTN cannot exceed 50 characters");

            RuleFor(x => x.Request.CreditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Credit limit must be greater than or equal to 0")
                .When(x => x.Request.CreditLimit.HasValue);
        }
    }
}