using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions
{
    public abstract class DepartmentDomainException : DomainException
    {
        protected DepartmentDomainException(string message) : base(message) { }
        protected DepartmentDomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DepartmentNotFoundException : DepartmentDomainException
    {
        public int DepartmentId { get; }
        public string? DepartmentName { get; }

        public DepartmentNotFoundException(int departmentId)
            : base($"Department with ID '{departmentId}' was not found.")
        {
            DepartmentId = departmentId;
        }

        public DepartmentNotFoundException(string departmentName)
            : base($"Department with name '{departmentName}' was not found.")
        {
            DepartmentName = departmentName;
        }

        public DepartmentNotFoundException(int departmentId, int branchId)
            : base($"Department with ID '{departmentId}' was not found for Branch ID '{branchId}'.")
        {
            DepartmentId = departmentId;
        }
    }

    public class DepartmentAlreadyExistsException : DepartmentDomainException
    {
        public string? DepartmentName { get; }
        public string? DepartmentCode { get; }

        public DepartmentAlreadyExistsException(string departmentName)
            : base($"A department with the name '{departmentName}' already exists.")
        {
            DepartmentName = departmentName;
        }

        public DepartmentAlreadyExistsException(string departmentName, int branchId)
            : base($"A department with the name '{departmentName}' already exists for Branch ID '{branchId}'.")
        {
            DepartmentName = departmentName;
            BranchId = branchId;
        }
        public int? BranchId { get; }

        public DepartmentAlreadyExistsException(string? departmentName, string? departmentCode, int branchId)
            : base($"A department with {(string.IsNullOrEmpty(departmentName) ? "" : $"name '{departmentName}'")} " +
                  $"{(string.IsNullOrEmpty(departmentCode) ? "" : $"or code '{departmentCode}'")} already exists for Branch ID '{branchId}'.")
        {
            DepartmentName = departmentName;
            DepartmentCode = departmentCode;
            BranchId = branchId;
        }
    }

    public class DepartmentDeactivationException : DepartmentDomainException
    {
        public int DepartmentId { get; }
        public string Reason { get; }

        public DepartmentDeactivationException(int departmentId, string reason)
            : base($"Cannot deactivate department with ID '{departmentId}': {reason}")
        {
            DepartmentId = departmentId;
            Reason = reason;
        }
    }

    public class DepartmentActivationException : DepartmentDomainException
    {
        public int DepartmentId { get; }
        public string Reason { get; }

        public DepartmentActivationException(int departmentId, string reason)
            : base($"Cannot activate department with ID '{departmentId}': {reason}")
        {
            DepartmentId = departmentId;
            Reason = reason;
        }
    }

    public class DepartmentDeletedException : DepartmentDomainException
    {
        public int DepartmentId { get; }

        public DepartmentDeletedException(int departmentId)
            : base($"Department with ID '{departmentId}' has been deleted.")
        {
            DepartmentId = departmentId;
        }
    }

    public class BranchNotFoundExceptionForDepartment : DepartmentDomainException
    {
        public int BranchId { get; }

        public BranchNotFoundExceptionForDepartment(int branchId)
            : base($"Branch with ID '{branchId}' does not exist. Cannot create department.")
        {
            BranchId = branchId;
        }
    }

    public class BranchInactiveException : DepartmentDomainException
    {
        public int BranchId { get; }
        public string BranchName { get; }

        public BranchInactiveException(int branchId, string branchName)
            : base($"Branch '{branchName}' (ID: {branchId}) is inactive. Cannot create department.")
        {
            BranchId = branchId;
            BranchName = branchName;
        }
    }

    public class DepartmentBusinessRuleException : DepartmentDomainException
    {
        public string RuleName { get; }

        public DepartmentBusinessRuleException(string ruleName, string message)
            : base($"Business rule '{ruleName}' violated: {message}")
        {
            RuleName = ruleName;
        }
    }
}
