using System;
using ERPWMS.Database.Models;
using MediatR; // ✅ Add this

namespace ERPWMS.Domain.Events
{
    /// <summary>
    /// Base class for company domain events
    /// </summary>
    public abstract class CompanyDomainEvent
    {
        public int CompanyId { get; set; }
        public DateTime OccurredOn { get; set; }
        public int? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Event raised when a company is registered
    /// </summary>
    public class CompanyRegisteredEvent : CompanyDomainEvent, INotification // ✅ Added INotification
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; }

        public CompanyRegisteredEvent(int companyId, string companyName, int userId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "CompanyRegistered";
        }
    }

    /// <summary>
    /// Event raised when a company profile is updated
    /// </summary>
    public class CompanyProfileUpdatedEvent : CompanyDomainEvent, INotification // ✅ Added INotification
    {
        public string OldCompanyName { get; set; } = string.Empty;
        public string NewCompanyName { get; set; } = string.Empty;
        public bool RegistrationNumberChanged { get; set; }
        public string? OldRegistrationNumber { get; set; }
        public string? NewRegistrationNumber { get; set; }

        public CompanyProfileUpdatedEvent(int companyId, string oldName, string newName, int userId)
        {
            CompanyId = companyId;
            OldCompanyName = oldName;
            NewCompanyName = newName;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "CompanyProfileUpdated";
        }
    }

    /// <summary>
    /// Event raised when a company is activated
    /// </summary>
    public class CompanyActivatedEvent : CompanyDomainEvent, INotification // ✅ Added INotification
    {
        public CompanyActivatedEvent(int companyId, int userId)
        {
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "CompanyActivated";
        }
    }

    /// <summary>
    /// Event raised when a company is deactivated
    /// </summary>
    public class CompanyDeactivatedEvent : CompanyDomainEvent, INotification // ✅ Added INotification
    {
        public string Reason { get; set; } = string.Empty;

        public CompanyDeactivatedEvent(int companyId, int userId, string reason)
        {
            CompanyId = companyId;
            UserId = userId;
            Reason = reason;
            OccurredOn = DateTime.UtcNow;
            EventType = "CompanyDeactivated";
        }
    }

    /// <summary>
    /// Event raised when a company is soft-deleted
    /// </summary>
    public class CompanySoftDeletedEvent : CompanyDomainEvent, INotification // ✅ Added INotification
    {
        public CompanySoftDeletedEvent(int companyId, int userId)
        {
            CompanyId = companyId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
            EventType = "CompanySoftDeleted";
        }
    }
}