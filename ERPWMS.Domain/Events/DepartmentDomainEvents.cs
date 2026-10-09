using System;

namespace ERPWMS.Domain.Events
{
    public abstract class DepartmentDomainEvent
    {
        public int DepartmentId { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public DateTime OccurredOn { get; set; }
        public int? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
    }

    public class DepartmentRegisteredEvent : DepartmentDomainEvent
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; }

        public DepartmentRegisteredEvent(int departmentId, int branchId, int companyId, string departmentName, int userId)
        {
            DepartmentId = departmentId;
            BranchId = branchId;
            CompanyId = companyId;
            DepartmentName = departmentName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DepartmentRegistered";
        }
    }

    public class DepartmentProfileUpdatedEvent : DepartmentDomainEvent
    {
        public string OldDepartmentName { get; set; } = string.Empty;
        public string NewDepartmentName { get; set; } = string.Empty;
        public bool DepartmentCodeChanged { get; set; }
        public string? OldDepartmentCode { get; set; }
        public string? NewDepartmentCode { get; set; }

        public DepartmentProfileUpdatedEvent(int departmentId, int branchId, int companyId, string oldName, string newName, int userId)
        {
            DepartmentId = departmentId;
            BranchId = branchId;
            CompanyId = companyId;
            OldDepartmentName = oldName;
            NewDepartmentName = newName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DepartmentProfileUpdated";
        }
    }

    public class DepartmentActivatedEvent : DepartmentDomainEvent
    {
        public DepartmentActivatedEvent(int departmentId, int branchId, int companyId, int userId)
        {
            DepartmentId = departmentId;
            BranchId = branchId;
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DepartmentActivated";
        }
    }

    public class DepartmentDeactivatedEvent : DepartmentDomainEvent
    {
        public string Reason { get; set; } = string.Empty;

        public DepartmentDeactivatedEvent(int departmentId, int branchId, int companyId, int userId, string reason)
        {
            DepartmentId = departmentId;
            BranchId = branchId;
            CompanyId = companyId;
            UserId = userId;
            Reason = reason;
            OccurredOn = DateTime.UtcNow;
            EventType = "DepartmentDeactivated";
        }
    }

    public class DepartmentSoftDeletedEvent : DepartmentDomainEvent
    {
        public DepartmentSoftDeletedEvent(int departmentId, int branchId, int companyId, int userId)
        {
            DepartmentId = departmentId;
            BranchId = branchId;
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DepartmentSoftDeleted";
        }
    }
}

