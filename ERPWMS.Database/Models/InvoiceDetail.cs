using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class InvoiceDetail
{
    public int InvoiceDetailId { get; set; }

    public int InvoiceId { get; set; }

    public int SalesDetailId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? DiscountPercent { get; set; }

    public decimal? TaxAmount { get; set; }

    public string? BatchNo { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SalesDetail SalesDetail { get; set; } = null!;

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();
}
