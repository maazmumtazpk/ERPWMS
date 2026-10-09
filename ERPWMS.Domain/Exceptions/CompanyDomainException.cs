using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions
{
   
    public abstract class CompanyDomainException : DomainException
    {
        protected CompanyDomainException(string message) : base(message) { }
        protected CompanyDomainException(string message, Exception innerException) : base(message, innerException) { }
    }

   
    public class CompanyNotFoundException : CompanyDomainException
    {
        public int CompanyId { get; }
        public string? CompanyName { get; }

        public CompanyNotFoundException(int companyId)
            : base($"Company with ID '{companyId}' was not found.")
        {
            CompanyId = companyId;
        }

        public CompanyNotFoundException(string companyName)
            : base($"Company with name '{companyName}' was not found.")
        {
            CompanyName = companyName;
        }
    }

  
    public class CompanyAlreadyExistsException : CompanyDomainException
    {
        public string? CompanyName { get; }
        public string? RegistrationNumber { get; }

        public CompanyAlreadyExistsException(string companyName)
            : base($"A company with the name '{companyName}' already exists.")
        {
            CompanyName = companyName;
        }

        public CompanyAlreadyExistsException(string? companyName, string? registrationNumber)
            : base($"A company with {(string.IsNullOrEmpty(companyName) ? "" : $"name '{companyName}'")} " +
                  $"{(string.IsNullOrEmpty(registrationNumber) ? "" : $"or registration number '{registrationNumber}'")} already exists.")
        {
            CompanyName = companyName;
            RegistrationNumber = registrationNumber;
        }
    }

    
    public class CompanyDeactivationException : CompanyDomainException
    {
        public int CompanyId { get; }
        public string Reason { get; }

        public CompanyDeactivationException(int companyId, string reason)
            : base($"Cannot deactivate company with ID '{companyId}': {reason}")
        {
            CompanyId = companyId;
            Reason = reason;
        }
    }

    public class CompanyActivationException : CompanyDomainException
    {
        public int CompanyId { get; }
        public string Reason { get; }

        public CompanyActivationException(int companyId, string reason)
            : base($"Cannot activate company with ID '{companyId}': {reason}")
        {
            CompanyId = companyId;
            Reason = reason;
        }
    }

  
    public class CompanyDeletedException : CompanyDomainException
    {
        public int CompanyId { get; }

        public CompanyDeletedException(int companyId)
            : base($"Company with ID '{companyId}' has been deleted.")
        {
            CompanyId = companyId;
        }
    }

    public class CompanyBusinessRuleException : CompanyDomainException
    {
        public string RuleName { get; }

        public CompanyBusinessRuleException(string ruleName, string message)
            : base($"Business rule '{ruleName}' violated: {message}")
        {
            RuleName = ruleName;
        }
    }
}