using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class PurchaseMaster
{
    public int PurchaseId { get; set; }

    public int SupplierId { get; set; }

    public int WarehouseId { get; set; }

    public string PurchaseOrderNo { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public DateOnly? ExpectedDeliveryDate { get; set; }

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

    public virtual ICollection<Grn> Grns { get; set; } = new List<Grn>();

    public virtual ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? CreatedByNavigation { get; set; }
}
