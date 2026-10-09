using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class StockTransferDetail
{
    public int TransferDetailId { get; set; }

    public int TransferId { get; set; }

    public int ProductId { get; set; }

    public int FromBinId { get; set; }

    public int ToBinId { get; set; }

    public int Quantity { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Bin FromBin { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Bin ToBin { get; set; } = null!;

    public virtual StockTransfer Transfer { get; set; } = null!;
}
