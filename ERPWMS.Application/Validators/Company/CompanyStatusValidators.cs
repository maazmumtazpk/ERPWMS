using ERPWMS.Application.DTOs.Company;
using FluentValidation;
using Humanizer;
using ERPWMS.Database.Models;
using static System.Net.Mime.MediaTypeNames;

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
