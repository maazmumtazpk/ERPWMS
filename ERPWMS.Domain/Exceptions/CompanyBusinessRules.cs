using System;
using System.Text.RegularExpressions;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Exceptions;

namespace ERPWMS.Domain.Rules
{
   
    public static class CompanyBusinessRules
    {
        private const int MAX_COMPANY_NAME_LENGTH = 200;
        private const int MIN_COMPANY_NAME_LENGTH = 2;
        private const int MAX_REGISTRATION_NUMBER_LENGTH = 50;
        private const int MAX_PHONE_LENGTH = 20;
        private static readonly Regex RegistrationNumberRegex = new(@"^[a-zA-Z0-9\-_]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

     
        public static void ValidateCompanyName(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new CompanyBusinessRuleException("CompanyNameRequired", "Company name is required.");

            if (companyName.Length > MAX_COMPANY_NAME_LENGTH)
                throw new CompanyBusinessRuleException("CompanyNameMaxLength",
                    $"Company name cannot exceed {MAX_COMPANY_NAME_LENGTH} characters.");

            if (companyName.Length < MIN_COMPANY_NAME_LENGTH)
                throw new CompanyBusinessRuleException("CompanyNameMinLength",
                    $"Company name must be at least {MIN_COMPANY_NAME_LENGTH} characters.");
        }

       
        public static void ValidateRegistrationNumber(string? registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                return;

            if (registrationNumber.Length > MAX_REGISTRATION_NUMBER_LENGTH)
                throw new CompanyBusinessRuleException("RegistrationNumberMaxLength",
                    $"Registration number cannot exceed {MAX_REGISTRATION_NUMBER_LENGTH} characters.");

            if (!RegistrationNumberRegex.IsMatch(registrationNumber))
                throw new CompanyBusinessRuleException("RegistrationNumberInvalidFormat",
                    "Registration number contains invalid characters. Only alphanumeric, dash, and underscore are allowed.");
        }

       
        public static void ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (!EmailRegex.IsMatch(email))
                throw new CompanyBusinessRuleException("EmailInvalidFormat", "Invalid email format.");
        }

       
        public static void ValidatePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return;

            if (phone.Length > MAX_PHONE_LENGTH)
                throw new CompanyBusinessRuleException("PhoneMaxLength",
                    $"Phone number cannot exceed {MAX_PHONE_LENGTH} characters.");
        }

       
        public static void ValidateUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            if (url.Length > 500)
                throw new CompanyBusinessRuleException("UrlMaxLength", "URL cannot exceed 500 characters.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                throw new CompanyBusinessRuleException("UrlInvalidFormat", "URL must be a valid absolute URL.");
        }

       
        public static void ValidateNotDeleted(Company company)
        {
            if (company.IsDeleted == true)
                throw new CompanyDeletedException(company.CompanyId);
        }

      
        public static void ValidateActive(Company company)
        {
            ValidateNotDeleted(company);

            if (company.IsActive != true)
                throw new CompanyActivationException(company.CompanyId, "Company is currently inactive.");
        }

      
        public static void ValidateCanDeactivate(Company company, bool hasActiveDependencies)
        {
            ValidateNotDeleted(company);

            if (company.IsActive != true)
                throw new CompanyDeactivationException(company.CompanyId, "Company is already inactive.");

            if (hasActiveDependencies)
                throw new CompanyDeactivationException(company.CompanyId,
                    "Company has active dependencies (branches, users, etc.).");
        }

       
        public static void ValidateCanActivate(Company company)
        {
            ValidateNotDeleted(company);

            if (company.IsActive == true)
                throw new CompanyActivationException(company.CompanyId, "Company is already active.");
        }

       
        public static void ValidateUniqueName(string companyName, bool isUnique)
        {
            if (!isUnique)
                throw new CompanyAlreadyExistsException(companyName);
        }

      
        public static void ValidateUniqueRegistrationNumber(string? registrationNumber, bool isUnique)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                return;

            if (!isUnique)
                throw new CompanyAlreadyExistsException(null, registrationNumber);
        }
    }
}