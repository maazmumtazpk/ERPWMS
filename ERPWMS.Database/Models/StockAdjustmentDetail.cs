using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class StockAdjustmentDetail
{
    public int AdjustmentDetailId { get; set; }

    public int AdjustmentId { get; set; }

    public int ProductId { get; set; }

    public int BinId { get; set; }

    public int Quantity { get; set; }

    public int PreviousQty { get; set; }

    public int NewQty { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual StockAdjustment Adjustment { get; set; } = null!;

    public virtual Bin Bin { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
