
using ERPWMS.Application.Common;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace ERPWMS.Infrastructure.Services
{
   
    public class ValidationService : IValidationService
    {
        private readonly IEnumerable<IValidator> _validators;

        public ValidationService(IEnumerable<IValidator> validators)
        {
            _validators = validators;
        }

        public IList<string> Validate(object obj)
        {
            var errors = new List<string>();

            foreach (var validator in _validators)
            {
                var validatorType = validator.GetType();
                var validatorInterface = validatorType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

                if (validatorInterface != null)
                {
                    var entityType = validatorInterface.GetGenericArguments()[0];
                    if (entityType.IsInstanceOfType(obj))
                    {
                        var validationContextType = typeof(ValidationContext<>).MakeGenericType(entityType);
                        var validationContext = Activator.CreateInstance(validationContextType, obj);
                        var validateMethod = validatorType.GetMethod("Validate", new[] { validationContextType });
                        var result = validateMethod?.Invoke(validator, new[] { validationContext }) as FluentValidation.Results.ValidationResult;

                        if (result != null && !result.IsValid)
                        {
                            errors.AddRange(result.Errors.Select(e => e.ErrorMessage));
                        }
                    }
                }
            }

            return errors;
        }

        public void ValidateAndThrow(object obj)
        {
            var errors = Validate(obj);
            if (errors.Any())
            {
                throw new FluentValidation.ValidationException(string.Join("; ", errors));
            }
        }

        public bool IsValid(object obj)
        {
            return !Validate(obj).Any();
        }
    }
}