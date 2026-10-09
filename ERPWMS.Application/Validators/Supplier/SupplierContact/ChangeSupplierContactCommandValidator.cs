using FluentValidation;
using ERPWMS.Application.Commands.Supplier.SupplierContact.ChangeSupplierContact;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Supplier.SupplierContact
{
    public class ChangeSupplierContactCommandValidator : AbstractValidator<ChangeSupplierContactCommand>
    {
        public ChangeSupplierContactCommandValidator()
        {
            RuleFor(x => x.ContactId)
                .NotEmpty().WithMessage("Invalid contact ID");

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
