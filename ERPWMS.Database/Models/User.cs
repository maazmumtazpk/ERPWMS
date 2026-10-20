using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string NormalizedUserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string NormalizedEmail { get; set; } = null!;

    public bool? EmailConfirmed { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? PhoneNumberConfirmed { get; set; }

    public bool? TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool? LockoutEnabled { get; set; }

    public int? AccessFailedCount { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? EmployeeCode { get; set; }

    public int? WarehouseId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public int? CompanyId { get; set; }




    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<CustomerComplaint> CustomerComplaintAssignedToNavigations { get; set; } = new List<CustomerComplaint>();

    public virtual ICollection<CustomerComplaint> CustomerComplaintEscalatedToNavigations { get; set; } = new List<CustomerComplaint>();

    public virtual ICollection<CustomerComplaint> CustomerComplaintResolvedByNavigations { get; set; } = new List<CustomerComplaint>();

    public virtual ICollection<CustomerInquiry> CustomerInquiryAssignedToNavigations { get; set; } = new List<CustomerInquiry>();

    public virtual ICollection<CustomerInquiry> CustomerInquiryResolvedByNavigations { get; set; } = new List<CustomerInquiry>();

    public virtual ICollection<CycleCount> CycleCountCountedByNavigations { get; set; } = new List<CycleCount>();

    public virtual ICollection<CycleCount> CycleCountVerifiedByNavigations { get; set; } = new List<CycleCount>();

    public virtual ICollection<DispatchMaster> DispatchMasters { get; set; } = new List<DispatchMaster>();

    public virtual ICollection<EmployeeLeaf> EmployeeLeaves { get; set; } = new List<EmployeeLeaf>();

    public virtual ICollection<EmployeeLeave> EmployeeLeavesNavigation { get; set; } = new List<EmployeeLeave>();

    public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; } = new List<EmployeePayroll>();

    public virtual ICollection<JournalVoucher> JournalVouchers { get; set; } = new List<JournalVoucher>();

    public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();

    public virtual ICollection<PickingList> PickingListAssignedToNavigations { get; set; } = new List<PickingList>();

    public virtual ICollection<PickingList> PickingListPickedByNavigations { get; set; } = new List<PickingList>();

    public virtual ICollection<PickingList> PickingListVerifiedByNavigations { get; set; } = new List<PickingList>();

    public virtual ICollection<ProductPricingHistory> ProductPricingHistories { get; set; } = new List<ProductPricingHistory>();

    public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();

    public virtual ICollection<ReceiptVoucher> ReceiptVouchers { get; set; } = new List<ReceiptVoucher>();

    public virtual ICollection<SalesReturn> SalesReturns { get; set; } = new List<SalesReturn>();

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockTransfer> StockTransferApprovedByNavigations { get; set; } = new List<StockTransfer>();

    public virtual ICollection<StockTransfer> StockTransferReceivedByNavigations { get; set; } = new List<StockTransfer>();

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public virtual Warehouse? Warehouse { get; set; }

    public virtual ICollection<WorkflowAssignment> WorkflowAssignments { get; set; } = new List<WorkflowAssignment>();

    public virtual ICollection<WorkflowDeadline> WorkflowDeadlines { get; set; } = new List<WorkflowDeadline>();

    public virtual ICollection<WorkflowInstanceLog> WorkflowInstanceLogs { get; set; } = new List<WorkflowInstanceLog>();

    public virtual ICollection<WorkflowNotification> WorkflowNotifications { get; set; } = new List<WorkflowNotification>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}

