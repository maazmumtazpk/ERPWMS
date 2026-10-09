using System;
using ERPWMS.Database.Models;
using MediatR; // ✅ Add this

namespace ERPWMS.Domain.Events
{
    /// <summary>
    /// Base class for branch domain events
    /// </summary>
    public abstract class BranchDomainEvent
    {
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public DateTime OccurredOn { get; set; }
        public int? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Event raised when a branch is registered
    /// </summary>
    public class BranchRegisteredEvent : BranchDomainEvent, INotification // ✅ Added INotification
    {
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }

        public BranchRegisteredEvent(int branchId, int companyId, string branchName, int userId)
        {
            BranchId = branchId;
            CompanyId = companyId;
            BranchName = branchName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "BranchRegistered";
        }
    }

    /// <summary>
    /// Event raised when a branch profile is updated
    /// </summary>
    public class BranchProfileUpdatedEvent : BranchDomainEvent, INotification // ✅ Added INotification
    {
        public string OldBranchName { get; set; } = string.Empty;
        public string NewBranchName { get; set; } = string.Empty;
        public bool BranchCodeChanged { get; set; }
        public string? OldBranchCode { get; set; }
        public string? NewBranchCode { get; set; }

        public BranchProfileUpdatedEvent(int branchId, int companyId, string oldName, string newName, int userId)
        {
            BranchId = branchId;
            CompanyId = companyId;
            OldBranchName = oldName;
            NewBranchName = newName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "BranchProfileUpdated";
        }
    }

    /// <summary>
    /// Event raised when a branch is activated
    /// </summary>
    public class BranchActivatedEvent : BranchDomainEvent, INotification // ✅ Added INotification
    {
        public BranchActivatedEvent(int branchId, int companyId, int userId)
        {
            BranchId = branchId;
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "BranchActivated";
        }
    }

    /// <summary>
    /// Event raised when a branch is deactivated
    /// </summary>
    public class BranchDeactivatedEvent : BranchDomainEvent, INotification // ✅ Added INotification
    {
        public string Reason { get; set; } = string.Empty;

        public BranchDeactivatedEvent(int branchId, int companyId, int userId, string reason)
        {
            BranchId = branchId;
            CompanyId = companyId;
            UserId = userId;
            Reason = reason;
            OccurredOn = DateTime.UtcNow;
            EventType = "BranchDeactivated";
        }
    }

    /// <summary>
    /// Event raised when a branch is soft-deleted
    /// </summary>
    public class BranchSoftDeletedEvent : BranchDomainEvent, INotification // ✅ Added INotification
    {
        public BranchSoftDeletedEvent(int branchId, int companyId, int userId)
        {
            BranchId = branchId;
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "BranchSoftDeleted";
        }
    }
}