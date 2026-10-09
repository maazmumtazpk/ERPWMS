using FluentValidation;
using ERPWMS.Application.Commands.Customer.CustomerContact.AddCustomerContact;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Customer.CustomerContact
{
    public class AddCustomerContactCommandValidator : AbstractValidator<AddCustomerContactCommand>
    {
        public AddCustomerContactCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Invalid customer ID");

            RuleFor(x => x.ContactName)
                .NotEmpty().WithMessage("Contact name is required")
                .MaximumLength(100).WithMessage("Contact name cannot exceed 100 characters");

            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Mobile is required")
                .MaximumLength(20).WithMessage("Mobile cannot exceed 20 characters");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters");

            RuleFor(x => x.Department)
                .MaximumLength(50).WithMessage("Department cannot exceed 50 characters");
        }
    }
}
