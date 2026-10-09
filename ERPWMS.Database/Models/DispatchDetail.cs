using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class DispatchDetail
{
    public int DispatchDetailId { get; set; }

    public int DispatchId { get; set; }

    public int SalesDetailId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? BatchNo { get; set; }

    public string? SerialNumbers { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual DispatchMaster Dispatch { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SalesDetail SalesDetail { get; set; } = null!;
}
