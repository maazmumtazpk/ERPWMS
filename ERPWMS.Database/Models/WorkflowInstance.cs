using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowInstance
{
    public int WorkflowInstId { get; set; }

    public int WorkflowDefId { get; set; }

    public int RecordId { get; set; }

    public string RecordType { get; set; } = null!;

    public int CurrentStageId { get; set; }

    public string? Status { get; set; }

    public DateTime? StartedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowStage CurrentStage { get; set; } = null!;

    public virtual ICollection<CustomerComplaint> CustomerComplaints { get; set; } = new List<CustomerComplaint>();

    public virtual ICollection<CustomerInquiry> CustomerInquiries { get; set; } = new List<CustomerInquiry>();

    public virtual ICollection<DeliveryChallan> DeliveryChallans { get; set; } = new List<DeliveryChallan>();

    public virtual ICollection<DispatchMaster> DispatchMasters { get; set; } = new List<DispatchMaster>();

    public virtual ICollection<EmployeeLeave> EmployeeLeaves { get; set; } = new List<EmployeeLeave>();

    public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; } = new List<EmployeePayroll>();

    public virtual ICollection<Grn> Grns { get; set; } = new List<Grn>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<JournalVoucher> JournalVouchers { get; set; } = new List<JournalVoucher>();

    public virtual ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();

    public virtual ICollection<PickingList> PickingLists { get; set; } = new List<PickingList>();

    public virtual ICollection<PurchaseMaster> PurchaseMasters { get; set; } = new List<PurchaseMaster>();

    public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();

    public virtual ICollection<ReceiptVoucher> ReceiptVouchers { get; set; } = new List<ReceiptVoucher>();

    public virtual ICollection<SalesMaster> SalesMasters { get; set; } = new List<SalesMaster>();

    public virtual ICollection<SalesReturn> SalesReturns { get; set; } = new List<SalesReturn>();

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockTransfer> StockTransfers { get; set; } = new List<StockTransfer>();

    public virtual ICollection<SupplierQuotation> SupplierQuotations { get; set; } = new List<SupplierQuotation>();

    public virtual ICollection<WorkflowDeadline> WorkflowDeadlines { get; set; } = new List<WorkflowDeadline>();

    public virtual WorkflowDefinition WorkflowDef { get; set; } = null!;

    public virtual ICollection<WorkflowInstanceLog> WorkflowInstanceLogs { get; set; } = new List<WorkflowInstanceLog>();

    public virtual ICollection<WorkflowNotification> WorkflowNotifications { get; set; } = new List<WorkflowNotification>();
}
