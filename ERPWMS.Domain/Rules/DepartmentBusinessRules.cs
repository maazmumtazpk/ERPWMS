using ERPWMS.Domain.Exceptions;
using ERPWMS.Database.Models;
using System.Text.RegularExpressions;

namespace ERPWMS.Domain.Rules
{
    public static class DepartmentBusinessRules
    {
        private const int MAX_DEPARTMENT_NAME_LENGTH = 200;
        private const int MIN_DEPARTMENT_NAME_LENGTH = 2;
        private const int MAX_DEPARTMENT_CODE_LENGTH = 20;

        private static readonly Regex DepartmentCodeRegex =
            new(@"^[a-zA-Z0-9\-_]+$", RegexOptions.Compiled);

        // =====================================================
        // DEPARTMENT NAME
        // =====================================================

        public static void ValidateDepartmentName(string departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
                throw new DepartmentBusinessRuleException(
                    "DepartmentNameRequired",
                    "Department name is required.");

            if (departmentName.Length > MAX_DEPARTMENT_NAME_LENGTH)
                throw new DepartmentBusinessRuleException(
                    "DepartmentNameMaxLength",
                    $"Department name cannot exceed {MAX_DEPARTMENT_NAME_LENGTH} characters.");

            if (departmentName.Length < MIN_DEPARTMENT_NAME_LENGTH)
                throw new DepartmentBusinessRuleException(
                    "DepartmentNameMinLength",
                    $"Department name must be at least {MIN_DEPARTMENT_NAME_LENGTH} characters.");
        }

        // =====================================================
        // DEPARTMENT CODE
        // =====================================================

        public static void ValidateDepartmentCode(string? departmentCode)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
                return;

            if (departmentCode.Length > MAX_DEPARTMENT_CODE_LENGTH)
                throw new DepartmentBusinessRuleException(
                    "DepartmentCodeMaxLength",
                    $"Department code cannot exceed {MAX_DEPARTMENT_CODE_LENGTH} characters.");

            if (!DepartmentCodeRegex.IsMatch(departmentCode))
                throw new DepartmentBusinessRuleException(
                    "DepartmentCodeInvalidFormat",
                    "Department code contains invalid characters. Only alphanumeric, dash, and underscore are allowed.");
        }

        // =====================================================
        // EMAIL VALIDATION
        // =====================================================

        public static void ValidateEmail(string? email)
        {
            // Email optional hai
            if (string.IsNullOrWhiteSpace(email))
                return;

            email = email.Trim();

            // Basic email validation
            var emailRegex = new Regex(
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (!emailRegex.IsMatch(email))
            {
                throw new DepartmentBusinessRuleException(
                    "DepartmentEmailInvalid",
                    "Invalid department email address.");
            }
        }

        // =====================================================
        // PHONE VALIDATION
        // =====================================================

        public static void ValidatePhone(string? phone)
        {
            // Phone optional hai
            if (string.IsNullOrWhiteSpace(phone))
                return;

            phone = phone.Trim();

            // Allows:
            // 03001234567
            // +923001234567
            // +92 300 1234567
            // 0300-1234567
            var phoneRegex = new Regex(
                @"^\+?[0-9\s\-\(\)]{7,20}$",
                RegexOptions.Compiled);

            if (!phoneRegex.IsMatch(phone))
            {
                throw new DepartmentBusinessRuleException(
                    "DepartmentPhoneInvalid",
                    "Invalid department phone number.");
            }
        }

        // =====================================================
        // ADDRESS VALIDATION
        // =====================================================

        public static void ValidateAddress(string? address)
        {
            // Address optional hai
            if (string.IsNullOrWhiteSpace(address))
                return;

            address = address.Trim();

            if (address.Length < 3)
            {
                throw new DepartmentBusinessRuleException(
                    "DepartmentAddressTooShort",
                    "Department address must be at least 3 characters.");
            }

            if (address.Length > 500)
            {
                throw new DepartmentBusinessRuleException(
                    "DepartmentAddressMaxLength",
                    "Department address cannot exceed 500 characters.");
            }
        }

        // =====================================================
        // BRANCH VALIDATION
        // =====================================================

        public static void ValidateBranch(Branch branch)
        {
            if (branch == null)
                throw new BranchNotFoundExceptionForDepartment(0);

            if (branch.IsDeleted == true)
                throw new BranchNotFoundExceptionForDepartment(
                    branch.BranchId);

            if (branch.IsActive != true)
                throw new BranchInactiveException(
                    branch.BranchId,
                    branch.BranchName);
        }

        // =====================================================
        // DEPARTMENT DELETE VALIDATION
        // =====================================================

        public static void ValidateNotDeleted(Department department)
        {
            if (department.IsDeleted == true)
                throw new DepartmentDeletedException(
                    department.DepartmentId);
        }

        // =====================================================
        // DEPARTMENT ACTIVE VALIDATION
        // =====================================================

        public static void ValidateActive(Department department)
        {
            ValidateNotDeleted(department);

            if (department.IsActive != true)
                throw new DepartmentActivationException(
                    department.DepartmentId,
                    "Department is currently inactive.");
        }

        // =====================================================
        // DEPARTMENT DEACTIVATE VALIDATION
        // =====================================================

        public static void ValidateCanDeactivate(
            Department department,
            bool hasActiveDependencies)
        {
            ValidateNotDeleted(department);

            if (department.IsActive != true)
                throw new DepartmentDeactivationException(
                    department.DepartmentId,
                    "Department is already inactive.");

            if (hasActiveDependencies)
                throw new DepartmentDeactivationException(
                    department.DepartmentId,
                    "Department has active dependencies (designations, employees, etc.).");
        }

        // =====================================================
        // DEPARTMENT ACTIVATE VALIDATION
        // =====================================================

        public static void ValidateCanActivate(
            Department department)
        {
            ValidateNotDeleted(department);

            if (department.IsActive == true)
                throw new DepartmentActivationException(
                    department.DepartmentId,
                    "Department is already active.");
        }

        // =====================================================
        // UNIQUE DEPARTMENT NAME
        // =====================================================

        public static void ValidateUniqueDepartmentName(
            string departmentName,
            int branchId,
            bool isUnique)
        {
            if (!isUnique)
                throw new DepartmentAlreadyExistsException(
                    departmentName,
                    branchId);
        }

        // =====================================================
        // UNIQUE DEPARTMENT CODE
        // =====================================================

        public static void ValidateUniqueDepartmentCode(
            string? departmentCode,
            int branchId,
            bool isUnique)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
                return;

            if (!isUnique)
                throw new DepartmentAlreadyExistsException(
                    null,
                    departmentCode,
                    branchId);
        }
    }
}