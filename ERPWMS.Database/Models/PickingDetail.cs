using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class PickingDetail
{
    public int PickingDetailId { get; set; }

    public int PickingId { get; set; }

    public int SalesDetailId { get; set; }

    public int ProductId { get; set; }

    public int BinId { get; set; }

    public int PickedQty { get; set; }

    public string? Status { get; set; }

    public string? BatchNo { get; set; }

    public string? SerialNumbers { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Bin Bin { get; set; } = null!;

    public virtual PickingList Picking { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SalesDetail SalesDetail { get; set; } = null!;
}
