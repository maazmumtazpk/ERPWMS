using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SalesReturnDetail
{
    public int ReturnDetailId { get; set; }

    public int ReturnId { get; set; }

    public int InvoiceDetailId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public string? BatchNo { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual InvoiceDetail InvoiceDetail { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SalesReturn Return { get; set; } = null!;
}
