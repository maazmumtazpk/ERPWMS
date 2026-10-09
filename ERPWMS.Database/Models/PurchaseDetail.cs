using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class PurchaseDetail
{
    public int DetailId { get; set; }

    public int PurchaseId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? DiscountPercent { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? ReceivedQty { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
    public decimal? DiscountAmount { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Grndetail> Grndetails { get; set; } = new List<Grndetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseMaster Purchase { get; set; } = null!;

    public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = new List<PurchaseReturnDetail>();
}
