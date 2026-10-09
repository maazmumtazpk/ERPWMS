using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions
{
    /// <summary>
    /// Base domain exception for Branch module
    /// </summary>
    public abstract class BranchDomainException : DomainException
    {
        protected BranchDomainException(string message) : base(message) { }
        protected BranchDomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when a branch is not found
    /// </summary>
    public class BranchNotFoundException : BranchDomainException
    {
        public int BranchId { get; }
        public string? BranchName { get; }
        public int? CompanyId { get; }

        public BranchNotFoundException(int branchId)
            : base($"Branch with ID '{branchId}' was not found.")
        {
            BranchId = branchId;
        }

        public BranchNotFoundException(string branchName)
            : base($"Branch with name '{branchName}' was not found.")
        {
            BranchName = branchName;
        }

        public BranchNotFoundException(int branchId, int companyId)
            : base($"Branch with ID '{branchId}' was not found for Company ID '{companyId}'.")
        {
            BranchId = branchId;
            CompanyId = companyId;
        }
    }

    /// <summary>
    /// Thrown when attempting to create a duplicate branch
    /// </summary>
    public class BranchAlreadyExistsException : BranchDomainException
    {
        public string? BranchName { get; }
        public string? BranchCode { get; }
        public int? CompanyId { get; }

        public BranchAlreadyExistsException(string branchName)
            : base($"A branch with the name '{branchName}' already exists.")
        {
            BranchName = branchName;
        }

        public BranchAlreadyExistsException(string branchName, int companyId)
            : base($"A branch with the name '{branchName}' already exists for Company ID '{companyId}'.")
        {
            BranchName = branchName;
            CompanyId = companyId;
        }

        public BranchAlreadyExistsException(string? branchName, string? branchCode, int companyId)
            : base($"A branch with {(string.IsNullOrEmpty(branchName) ? "" : $"name '{branchName}'")} " +
                  $"{(string.IsNullOrEmpty(branchCode) ? "" : $"or code '{branchCode}'")} already exists for Company ID '{companyId}'.")
        {
            BranchName = branchName;
            BranchCode = branchCode;
            CompanyId = companyId;
        }
    }

    /// <summary>
    /// Thrown when attempting to deactivate a branch with active dependencies
    /// </summary>
    public class BranchDeactivationException : BranchDomainException
    {
        public int BranchId { get; }
        public string Reason { get; }

        public BranchDeactivationException(int branchId, string reason)
            : base($"Cannot deactivate branch with ID '{branchId}': {reason}")
        {
            BranchId = branchId;
            Reason = reason;
        }
    }

    /// <summary>
    /// Thrown when attempting to activate a branch that is already active or soft-deleted
    /// </summary>
    public class BranchActivationException : BranchDomainException
    {
        public int BranchId { get; }
        public string Reason { get; }

        public BranchActivationException(int branchId, string reason)
            : base($"Cannot activate branch with ID '{branchId}': {reason}")
        {
            BranchId = branchId;
            Reason = reason;
        }
    }

    /// <summary>
    /// Thrown when attempting to perform an operation on a soft-deleted branch
    /// </summary>
    public class BranchDeletedException : BranchDomainException
    {
        public int BranchId { get; }

        public BranchDeletedException(int branchId)
            : base($"Branch with ID '{branchId}' has been deleted.")
        {
            BranchId = branchId;
        }
    }

    /// <summary>
    /// Thrown when a company does not exist for branch creation
    /// </summary>
    public class CompanyNotFoundExceptionForBranch : BranchDomainException
    {
        public int CompanyId { get; }

        public CompanyNotFoundExceptionForBranch(int companyId)
            : base($"Company with ID '{companyId}' does not exist. Cannot create branch.")
        {
            CompanyId = companyId;
        }
    }

    /// <summary>
    /// Thrown when a company is inactive and branch cannot be created
    /// </summary>
    public class CompanyInactiveException : BranchDomainException
    {
        public int CompanyId { get; }
        public string CompanyName { get; }

        public CompanyInactiveException(int companyId, string companyName)
            : base($"Company '{companyName}' (ID: {companyId}) is inactive. Cannot create branch.")
        {
            CompanyId = companyId;
            CompanyName = companyName;
        }
    }

    /// <summary>
    /// Thrown when a business rule validation fails
    /// </summary>
    public class BranchBusinessRuleException : BranchDomainException
    {
        public string RuleName { get; }

        public BranchBusinessRuleException(string ruleName, string message)
            : base($"Business rule '{ruleName}' violated: {message}")
        {
            RuleName = ruleName;
        }
    }
}