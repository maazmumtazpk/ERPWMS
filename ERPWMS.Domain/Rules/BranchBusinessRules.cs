using ERPWMS.Domain.Exceptions;
using ERPWMS.Database.Models;
using System;
using System.Text.RegularExpressions;

namespace ERPWMS.Domain.Rules
{
    /// <summary>
    /// Business rules for Branch entity
    /// </summary>
    public static class BranchBusinessRules
    {
        private const int MAX_BRANCH_NAME_LENGTH = 200;
        private const int MIN_BRANCH_NAME_LENGTH = 2;
        private const int MAX_BRANCH_CODE_LENGTH = 20;
        private const int MAX_PHONE_LENGTH = 20;
        private const int MAX_ADDRESS_LENGTH = 500;
        private const int MAX_EMAIL_LENGTH = 100;

        private static readonly Regex BranchCodeRegex = new(@"^[a-zA-Z0-9\-_]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        /// <summary>
        /// Validates that the branch name is not empty and meets length requirements
        /// </summary>
        public static void ValidateBranchName(string branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                throw new BranchBusinessRuleException("BranchNameRequired", "Branch name is required.");

            if (branchName.Length > MAX_BRANCH_NAME_LENGTH)
                throw new BranchBusinessRuleException("BranchNameMaxLength",
                    $"Branch name cannot exceed {MAX_BRANCH_NAME_LENGTH} characters.");

            if (branchName.Length < MIN_BRANCH_NAME_LENGTH)
                throw new BranchBusinessRuleException("BranchNameMinLength",
                    $"Branch name must be at least {MIN_BRANCH_NAME_LENGTH} characters.");
        }

        /// <summary>
        /// Validates that the branch code meets format requirements if provided
        /// </summary>
        public static void ValidateBranchCode(string? branchCode)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
                return;

            if (branchCode.Length > MAX_BRANCH_CODE_LENGTH)
                throw new BranchBusinessRuleException("BranchCodeMaxLength",
                    $"Branch code cannot exceed {MAX_BRANCH_CODE_LENGTH} characters.");

            if (!BranchCodeRegex.IsMatch(branchCode))
                throw new BranchBusinessRuleException("BranchCodeInvalidFormat",
                    "Branch code contains invalid characters. Only alphanumeric, dash, and underscore are allowed.");
        }

        /// <summary>
        /// Validates email format if provided
        /// </summary>
        public static void ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (email.Length > MAX_EMAIL_LENGTH)
                throw new BranchBusinessRuleException("EmailMaxLength",
                    $"Email cannot exceed {MAX_EMAIL_LENGTH} characters.");

            if (!EmailRegex.IsMatch(email))
                throw new BranchBusinessRuleException("EmailInvalidFormat", "Invalid email format.");
        }

        /// <summary>
        /// Validates phone number format if provided
        /// </summary>
        public static void ValidatePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return;

            if (phone.Length > MAX_PHONE_LENGTH)
                throw new BranchBusinessRuleException("PhoneMaxLength",
                    $"Phone number cannot exceed {MAX_PHONE_LENGTH} characters.");
        }

        /// <summary>
        /// Validates address length if provided
        /// </summary>
        public static void ValidateAddress(string? address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return;

            if (address.Length > MAX_ADDRESS_LENGTH)
                throw new BranchBusinessRuleException("AddressMaxLength",
                    $"Address cannot exceed {MAX_ADDRESS_LENGTH} characters.");
        }

        /// <summary>
        /// Validates that the company exists and is active
        /// </summary>
        public static void ValidateCompany(Company company)
        {
            if (company == null)
                throw new CompanyNotFoundExceptionForBranch(0);

            if (company.IsDeleted == true)
                throw new CompanyNotFoundExceptionForBranch(company.CompanyId);

            if (company.IsActive != true)
                throw new CompanyInactiveException(company.CompanyId, company.CompanyName);
        }

        /// <summary>
        /// Validates that the branch is not in a deleted state
        /// </summary>
        public static void ValidateNotDeleted(Branch branch)
        {
            if (branch.IsDeleted == true)
                throw new BranchDeletedException(branch.BranchId);
        }

        /// <summary>
        /// Validates that the branch is active before performing certain operations
        /// </summary>
        public static void ValidateActive(Branch branch)
        {
            ValidateNotDeleted(branch);

            if (branch.IsActive != true)
                throw new BranchActivationException(branch.BranchId, "Branch is currently inactive.");
        }

        /// <summary>
        /// Validates that the branch can be deactivated
        /// </summary>
        public static void ValidateCanDeactivate(Branch branch, bool hasActiveDependencies)
        {
            ValidateNotDeleted(branch);

            if (branch.IsActive != true)
                throw new BranchDeactivationException(branch.BranchId, "Branch is already inactive.");

            if (hasActiveDependencies)
                throw new BranchDeactivationException(branch.BranchId,
                    "Branch has active dependencies (employees, inventory, etc.).");
        }

        /// <summary>
        /// Validates that the branch can be activated
        /// </summary>
        public static void ValidateCanActivate(Branch branch)
        {
            ValidateNotDeleted(branch);

            if (branch.IsActive == true)
                throw new BranchActivationException(branch.BranchId, "Branch is already active.");
        }

        /// <summary>
        /// Validates that the branch name is unique within the company
        /// </summary>
        public static void ValidateUniqueBranchName(string branchName, int companyId, bool isUnique)
        {
            if (!isUnique)
                throw new BranchAlreadyExistsException(branchName, companyId);
        }

        /// <summary>
        /// Validates that the branch code is unique if provided
        /// </summary>
        public static void ValidateUniqueBranchCode(string? branchCode, int companyId, bool isUnique)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
                return;

            if (!isUnique)
                throw new BranchAlreadyExistsException(null, branchCode, companyId);
        }
    }
}