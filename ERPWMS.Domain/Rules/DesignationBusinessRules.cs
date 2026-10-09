using ERPWMS.Domain.Exceptions;
using ERPWMS.Database.Models;
using System.Text.RegularExpressions;

namespace ERPWMS.Domain.Rules
{
    public static class DesignationBusinessRules
    {
        private const int MAX_DESIGNATION_NAME_LENGTH = 200;
        private const int MIN_DESIGNATION_NAME_LENGTH = 2;

        public static void ValidateDesignationName(string designationName)
        {
            if (string.IsNullOrWhiteSpace(designationName))
                throw new DesignationBusinessRuleException("DesignationNameRequired", "Designation name is required.");

            if (designationName.Length > MAX_DESIGNATION_NAME_LENGTH)
                throw new DesignationBusinessRuleException("DesignationNameMaxLength",
                    $"Designation name cannot exceed {MAX_DESIGNATION_NAME_LENGTH} characters.");

            if (designationName.Length < MIN_DESIGNATION_NAME_LENGTH)
                throw new DesignationBusinessRuleException("DesignationNameMinLength",
                    $"Designation name must be at least {MIN_DESIGNATION_NAME_LENGTH} characters.");
        }

        public static void ValidateRankLevel(int? rankLevel)
        {
            if (rankLevel.HasValue && rankLevel < 0)
                throw new DesignationBusinessRuleException("RankLevelInvalid", "Rank level cannot be negative.");
        }

        public static void ValidateDepartment(Department department)
        {
            if (department == null)
                throw new DepartmentNotFoundExceptionForDesignation(0);

            if (department.IsDeleted == true)
                throw new DepartmentNotFoundExceptionForDesignation(department.DepartmentId);

            if (department.IsActive != true)
                throw new DepartmentInactiveException(department.DepartmentId, department.DepartmentName);
        }

        public static void ValidateNotDeleted(Designation designation)
        {
            if (designation.IsDeleted == true)
                throw new DesignationDeletedException(designation.DesignationId);
        }

        public static void ValidateActive(Designation designation)
        {
            ValidateNotDeleted(designation);

            if (designation.IsActive != true)
                throw new DesignationActivationException(designation.DesignationId, "Designation is currently inactive.");
        }

        public static void ValidateCanDeactivate(Designation designation, bool hasActiveDependencies)
        {
            ValidateNotDeleted(designation);

            if (designation.IsActive != true)
                throw new DesignationDeactivationException(designation.DesignationId, "Designation is already inactive.");

            if (hasActiveDependencies)
                throw new DesignationDeactivationException(designation.DesignationId,
                    "Designation has active dependencies (employees, etc.).");
        }

        public static void ValidateCanActivate(Designation designation)
        {
            ValidateNotDeleted(designation);

            if (designation.IsActive == true)
                throw new DesignationActivationException(designation.DesignationId, "Designation is already active.");
        }

        public static void ValidateUniqueDesignationName(string designationName, int departmentId, bool isUnique)
        {
            if (!isUnique)
                throw new DesignationAlreadyExistsException(designationName, departmentId);
        }
    }
}