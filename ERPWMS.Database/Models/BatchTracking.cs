using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class BatchTracking
{
    public int BatchId { get; set; }

    public int ProductId { get; set; }

    public int GrndetailId { get; set; }

    public string BatchNo { get; set; } = null!;

    public DateOnly ExpiryDate { get; set; }

    public int Quantity { get; set; }

    public int RemainingQty { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Grndetail Grndetail { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
