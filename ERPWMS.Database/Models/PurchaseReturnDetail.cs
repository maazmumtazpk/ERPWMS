using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class PurchaseReturnDetail
{
    public int ReturnDetailId { get; set; }

    public int ReturnId { get; set; }

    public int PurchaseDetailId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseDetail PurchaseDetail { get; set; } = null!;

    public virtual PurchaseReturn Return { get; set; } = null!;
}
