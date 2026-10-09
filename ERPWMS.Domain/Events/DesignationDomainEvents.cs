using System;

namespace ERPWMS.Domain.Events
{
    public abstract class DesignationDomainEvent
    {
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public DateTime OccurredOn { get; set; }
        public int? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
    }

    public class DesignationRegisteredEvent : DesignationDomainEvent
    {
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }

        public DesignationRegisteredEvent(int designationId, int departmentId, int companyId, string designationName, int userId)
        {
            DesignationId = designationId;
            DepartmentId = departmentId;
            CompanyId = companyId;
            DesignationName = designationName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DesignationRegistered";
        }
    }

    public class DesignationProfileUpdatedEvent : DesignationDomainEvent
    {
        public string OldDesignationName { get; set; } = string.Empty;
        public string NewDesignationName { get; set; } = string.Empty;
        public int? OldRankLevel { get; set; }
        public int? NewRankLevel { get; set; }

        public DesignationProfileUpdatedEvent(int designationId, int departmentId, int companyId, string oldName, string newName, int userId)
        {
            DesignationId = designationId;
            DepartmentId = departmentId;
            CompanyId = companyId;
            OldDesignationName = oldName;
            NewDesignationName = newName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DesignationProfileUpdated";
        }
    }

    public class DesignationActivatedEvent : DesignationDomainEvent
    {
        public DesignationActivatedEvent(int designationId, int departmentId, int companyId, int userId)
        {
            DesignationId = designationId;
            DepartmentId = departmentId;
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DesignationActivated";
        }
    }

    public class DesignationDeactivatedEvent : DesignationDomainEvent
    {
        public string Reason { get; set; } = string.Empty;

        public DesignationDeactivatedEvent(int designationId, int departmentId, int companyId, int userId, string reason)
        {
            DesignationId = designationId;
            DepartmentId = departmentId;
            CompanyId = companyId;
            UserId = userId;
            Reason = reason;
            OccurredOn = DateTime.UtcNow;
            EventType = "DesignationDeactivated";
        }
    }

    public class DesignationSoftDeletedEvent : DesignationDomainEvent
    {
        public DesignationSoftDeletedEvent(int designationId, int departmentId, int companyId, int userId)
        {
            DesignationId = designationId;
            DepartmentId = departmentId;
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "DesignationSoftDeleted";
        }
    }
}
