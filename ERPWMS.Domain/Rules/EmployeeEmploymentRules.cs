using System;
using ERPWMS.Domain.Exceptions.Employee;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Rules
{
    public static class EmployeeEmploymentRules
    {
        public static void ValidateHireAndTerminationDates(
            DateOnly hireDate,
            DateOnly? terminationDate)
        {
            if (terminationDate.HasValue &&
                terminationDate < hireDate)
            {
                throw new EmployeeEmploymentException(
                    "Termination date cannot be before hire date.");
            }
        }

        public static void ValidateActiveStatusTransition(
            bool currentIsActive,
            bool newIsActive,
            DateOnly? terminationDate)
        {
            if (currentIsActive &&
                !newIsActive &&
                !terminationDate.HasValue)
            {
                throw new EmployeeEmploymentException(
                    "Cannot set employee to inactive without a termination date.");
            }

            if (!currentIsActive &&
                newIsActive &&
                terminationDate.HasValue)
            {
                throw new EmployeeEmploymentException(
                    "Cannot reactivate a terminated employee.");
            }
        }

        public static void ValidateDepartmentDesignationBranchExistence(
            int departmentId,
            int designationId,
            int branchId)
        {
            if (departmentId <= 0 ||
                designationId <= 0 ||
                branchId <= 0)
            {
                throw new EmployeeEmploymentException(
                    "Department, Designation, and Branch must have valid IDs.");
            }
        }
    }
}