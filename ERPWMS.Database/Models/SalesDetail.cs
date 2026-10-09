using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SalesDetail
{
    public int DetailId { get; set; }

    public int SalesId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? DiscountPercent { get; set; }

    public string? BatchNo { get; set; }

    public int? AllocatedQty { get; set; }

    public int? PickedQty { get; set; }

    public int? ShippedQty { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
    public int SalesDetailId { get; set; }
    public decimal? DiscountAmount { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<DispatchDetail> DispatchDetails { get; set; } = new List<DispatchDetail>();

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ICollection<PickingDetail> PickingDetails { get; set; } = new List<PickingDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual SalesMaster Sales { get; set; } = null!;

    public virtual ICollection<SerialNumber> SerialNumbers { get; set; } = new List<SerialNumber>();
}
