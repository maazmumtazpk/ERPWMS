using ERPWMS.Application.DTOs.Company;
using FluentValidation;
using ERPWMS.Database.Models;
namespace ERPWMS.Application.Validators.Company
{
	public class ActivateCompanyValidator : AbstractValidator<ActivateCompanyCommand>
	{
		public ActivateCompanyValidator()
		{
			RuleFor(x => x.CompanyId)
				.NotEmpty()
				.WithMessage("A valid company ID is required.");
		}
	}

	public class DeactivateCompanyValidator : AbstractValidator<DeactivateCompanyCommand>
	{
		public DeactivateCompanyValidator()
		{
			RuleFor(x => x.CompanyId)
				.NotEmpty()
				.WithMessage("A valid company ID is required.");

			RuleFor(x => x.Reason)
				.MaximumLength(500)
				.WithMessage("Reason cannot exceed 500 characters.")
				.When(x => !string.IsNullOrEmpty(x.Reason));
		}
	}
}

