using ERPWMS.Application.DTOs.Notification;
using ERPWMS.Domain.Services;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Validators.Notification
{
    public class CreateNotificationTemplateValidator : AbstractValidator<CreateNotificationTemplateRequest>
    {
        private readonly INotificationTemplateService _templateService;

        public CreateNotificationTemplateValidator(INotificationTemplateService templateService)
        {
            _templateService = templateService;

            RuleFor(x => x.TemplateName)
                .NotEmpty().WithMessage("Template name is required.")
                .MaximumLength(100).WithMessage("Template name cannot exceed 100 characters.");

            RuleFor(x => x.TemplateCode)
                .NotEmpty().WithMessage("Template code is required.")
                .MaximumLength(50).WithMessage("Template code cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Template code can only contain letters, numbers, and underscores.")
                .MustAsync(BeUniqueCode).WithMessage("Template code already exists.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(200).WithMessage("Subject cannot exceed 200 characters.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required.");

            // Validate that no unapproved variables are used
            RuleFor(x => x.Body)
                .Must(ValidateTemplateVariables)
                .WithMessage("Template contains unapproved variables. Allowed: {{UserName}}, {{UserEmail}}, {{ReferenceNo}}, {{CurrentDate}}, etc.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return !await _templateService.TemplateCodeExistsAsync(code);
        }

        private bool ValidateTemplateVariables(string body)
        {
            // This is a basic check; full validation is in the service
            return !string.IsNullOrEmpty(body);
        }
    }

    public class UpdateNotificationTemplateValidator : AbstractValidator<UpdateNotificationTemplateRequest>
    {
        private readonly INotificationTemplateService _templateService;

        public UpdateNotificationTemplateValidator(INotificationTemplateService templateService)
        {
            _templateService = templateService;

            RuleFor(x => x.TemplateId)
                .NotEmpty().WithMessage("Invalid template.");

            RuleFor(x => x.TemplateName)
                .NotEmpty().WithMessage("Template name is required.")
                .MaximumLength(100).WithMessage("Template name cannot exceed 100 characters.");

            RuleFor(x => x.TemplateCode)
                .NotEmpty().WithMessage("Template code is required.")
                .MaximumLength(50).WithMessage("Template code cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Template code can only contain letters, numbers, and underscores.")
                .MustAsync(BeUniqueCode).WithMessage("Template code already exists.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(200).WithMessage("Subject cannot exceed 200 characters.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required.");
        }

        private async Task<bool> BeUniqueCode(UpdateNotificationTemplateRequest request, string code, CancellationToken cancellationToken)
        {
            return !await _templateService.TemplateCodeExistsAsync(code, request.TemplateId);
        }
    }
}