using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Grndetail
{
    public int GrndetailId { get; set; }

    public int GrnId { get; set; }

    public int PurchaseDetailId { get; set; }

    public int ProductId { get; set; }

    public int BinId { get; set; }

    public int ReceivedQty { get; set; }

    public int AcceptedQty { get; set; }

    public int? RejectedQty { get; set; }

    public string? RejectionReason { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? SerialNumbers { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateOnly? MfgDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<BatchTracking> BatchTrackings { get; set; } = new List<BatchTracking>();

    public virtual Bin Bin { get; set; } = null!;

    public virtual Grn Grn { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseDetail PurchaseDetail { get; set; } = null!;

    public virtual ICollection<SerialNumber> SerialNumbersNavigation { get; set; } = new List<SerialNumber>();
}
