using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SalesMaster
{
    public int SalesId { get; set; }

    public int CustomerId { get; set; }

    public int WarehouseId { get; set; }

    public string SalesOrderNo { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal NetAmount { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public string? ApprovalStatus { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<DeliveryChallan> DeliveryChallans { get; set; } = new List<DeliveryChallan>();

    public virtual ICollection<DispatchMaster> DispatchMasters { get; set; } = new List<DispatchMaster>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<PickingList> PickingLists { get; set; } = new List<PickingList>();

    public virtual ICollection<ReceiptVoucher> ReceiptVouchers { get; set; } = new List<ReceiptVoucher>();

    public virtual ICollection<SalesDetail> SalesDetails { get; set; } = new List<SalesDetail>();

    public virtual ICollection<SalesReturn> SalesReturns { get; set; } = new List<SalesReturn>();

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
    public bool? IsActive { get; set; }
}
