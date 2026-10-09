using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowStage
{
    public int StageId { get; set; }

    public int WorkflowDefId { get; set; }

    public string StageName { get; set; } = null!;

    public string StageCode { get; set; } = null!;

    public int StageOrder { get; set; }

    public bool? IsStart { get; set; }

    public bool? IsEnd { get; set; }

    public int? SladeadlineHours { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<CustomerComplaint> CustomerComplaints { get; set; } = new List<CustomerComplaint>();

    public virtual ICollection<CustomerInquiry> CustomerInquiries { get; set; } = new List<CustomerInquiry>();

    public virtual ICollection<DeliveryChallan> DeliveryChallans { get; set; } = new List<DeliveryChallan>();

    public virtual ICollection<DispatchMaster> DispatchMasters { get; set; } = new List<DispatchMaster>();

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

    public virtual ICollection<WorkflowAssignment> WorkflowAssignments { get; set; } = new List<WorkflowAssignment>();

    public virtual ICollection<WorkflowCondition> WorkflowConditionStageFroms { get; set; } = new List<WorkflowCondition>();

    public virtual ICollection<WorkflowCondition> WorkflowConditionStageTos { get; set; } = new List<WorkflowCondition>();

    public virtual ICollection<WorkflowDeadline> WorkflowDeadlines { get; set; } = new List<WorkflowDeadline>();

    public virtual WorkflowDefinition WorkflowDef { get; set; } = null!;

    public virtual ICollection<WorkflowInstanceLog> WorkflowInstanceLogStageFroms { get; set; } = new List<WorkflowInstanceLog>();

    public virtual ICollection<WorkflowInstanceLog> WorkflowInstanceLogStageTos { get; set; } = new List<WorkflowInstanceLog>();

    public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();

    public virtual ICollection<WorkflowTransition> WorkflowTransitionStageFroms { get; set; } = new List<WorkflowTransition>();

    public virtual ICollection<WorkflowTransition> WorkflowTransitionStageTos { get; set; } = new List<WorkflowTransition>();
}
