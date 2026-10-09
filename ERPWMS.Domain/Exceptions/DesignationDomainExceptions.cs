using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions
{
    public abstract class DesignationDomainException : DomainException
    {
        protected DesignationDomainException(string message) : base(message) { }
        protected DesignationDomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DesignationNotFoundException : DesignationDomainException
    {
        public int DesignationId { get; }
        public string? DesignationName { get; }

        public DesignationNotFoundException(int designationId)
            : base($"Designation with ID '{designationId}' was not found.")
        {
            DesignationId = designationId;
        }

        public DesignationNotFoundException(string designationName)
            : base($"Designation with name '{designationName}' was not found.")
        {
            DesignationName = designationName;
        }

        public DesignationNotFoundException(int designationId, int departmentId)
            : base($"Designation with ID '{designationId}' was not found for Department ID '{departmentId}'.")
        {
            DesignationId = designationId;
        }
    }

    public class DesignationAlreadyExistsException : DesignationDomainException
    {
        public string? DesignationName { get; }
        public int? DepartmentId { get; }

        public DesignationAlreadyExistsException(string designationName)
            : base($"A designation with the name '{designationName}' already exists.")
        {
            DesignationName = designationName;
        }

        public DesignationAlreadyExistsException(string designationName, int departmentId)
            : base($"A designation with the name '{designationName}' already exists for Department ID '{departmentId}'.")
        {
            DesignationName = designationName;
            DepartmentId = departmentId;
        }
    }

    public class DesignationDeactivationException : DesignationDomainException
    {
        public int DesignationId { get; }
        public string Reason { get; }

        public DesignationDeactivationException(int designationId, string reason)
            : base($"Cannot deactivate designation with ID '{designationId}': {reason}")
        {
            DesignationId = designationId;
            Reason = reason;
        }
    }

    public class DesignationActivationException : DesignationDomainException
    {
        public int DesignationId { get; }
        public string Reason { get; }

        public DesignationActivationException(int designationId, string reason)
            : base($"Cannot activate designation with ID '{designationId}': {reason}")
        {
            DesignationId = designationId;
            Reason = reason;
        }
    }

    public class DesignationDeletedException : DesignationDomainException
    {
        public int DesignationId { get; }

        public DesignationDeletedException(int designationId)
            : base($"Designation with ID '{designationId}' has been deleted.")
        {
            DesignationId = designationId;
        }
    }

    public class DepartmentNotFoundExceptionForDesignation : DesignationDomainException
    {
        public int DepartmentId { get; }

        public DepartmentNotFoundExceptionForDesignation(int departmentId)
            : base($"Department with ID '{departmentId}' does not exist. Cannot create designation.")
        {
            DepartmentId = departmentId;
        }
    }

    public class DepartmentInactiveException : DesignationDomainException
    {
        public int DepartmentId { get; }
        public string DepartmentName { get; }

        public DepartmentInactiveException(int departmentId, string departmentName)
            : base($"Department '{departmentName}' (ID: {departmentId}) is inactive. Cannot create designation.")
        {
            DepartmentId = departmentId;
            DepartmentName = departmentName;
        }
    }

    public class DesignationBusinessRuleException : DesignationDomainException
    {
        public string RuleName { get; }

        public DesignationBusinessRuleException(string ruleName, string message)
            : base($"Business rule '{ruleName}' violated: {message}")
        {
            RuleName = ruleName;
        }
    }
}