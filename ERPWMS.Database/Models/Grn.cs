using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Grn
{
    public int GrnId { get; set; }

    public int PurchaseId { get; set; }

    public int WarehouseId { get; set; }

    public string GrnNo { get; set; } = null!;

    public DateTime? ReceiptDate { get; set; }

    public int TotalReceivedQty { get; set; }

    public int TotalAcceptedQty { get; set; }

    public int? TotalRejectedQty { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual ICollection<Grndetail> Grndetails { get; set; } = new List<Grndetail>();

    public virtual PurchaseMaster Purchase { get; set; } = null!;

    public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
