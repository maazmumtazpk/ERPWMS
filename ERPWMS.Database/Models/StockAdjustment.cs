using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class StockAdjustment
{
    public int AdjustmentId { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public int BinId { get; set; }

    public string AdjustmentType { get; set; } = null!;

    public int Quantity { get; set; }

    public int PreviousQty { get; set; }

    public int NewQty { get; set; }

    public string Reason { get; set; } = null!;

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual Bin Bin { get; set; } = null!;

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<StockAdjustmentDetail> StockAdjustmentDetails { get; set; } = new List<StockAdjustmentDetail>();

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
    public bool? IsActive { get; set; }
}
