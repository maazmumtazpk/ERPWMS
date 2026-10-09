using ERPWMS.Application.DTOs.System;
using ERPWMS.Domain.Services;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Validators.System
{
    public class UpdateSystemSettingValidator : AbstractValidator<UpdateSystemSettingRequest>
    {
        private readonly ISystemSettingService _settingService;

        public UpdateSystemSettingValidator(ISystemSettingService settingService)
        {
            _settingService = settingService;

            RuleFor(x => x.SettingId)
                .NotEmpty().WithMessage("Invalid setting.");

            RuleFor(x => x.SettingKey)
                .NotEmpty().WithMessage("Setting key is required.")
                .MaximumLength(100).WithMessage("Setting key cannot exceed 100 characters.")
                .MustAsync(BeUniqueKey).WithMessage("Setting key already exists.");

            RuleFor(x => x.SettingValue)
                .NotEmpty().WithMessage("Setting value is required.");

            // DataType validation
            RuleFor(x => x)
                .Must(ValidateDataType).WithMessage("Setting value does not match the specified data type.");
        }

        private async Task<bool> BeUniqueKey(UpdateSystemSettingRequest request, string key, CancellationToken cancellationToken)
        {
            return !await _settingService.SettingExistsAsync(key, request.SettingId);
        }

        private bool ValidateDataType(UpdateSystemSettingRequest request)
        {
            if (string.IsNullOrEmpty(request.DataType))
                return true;

            return request.DataType.ToLower() switch
            {
                "string" => true,
                "integer" => int.TryParse(request.SettingValue, out _),
                "boolean" => bool.TryParse(request.SettingValue, out _),
                "decimal" => decimal.TryParse(request.SettingValue, out _),
                "datetime" => DateTime.TryParse(request.SettingValue, out _),
                _ => true
            };
        }
    }
}