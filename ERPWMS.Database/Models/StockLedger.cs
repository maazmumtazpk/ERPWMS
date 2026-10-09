using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class StockLedger
{
    public int LedgerId { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int BinId { get; set; }

    public string TransactionType { get; set; } = null!;

    public string ReferenceDocNo { get; set; } = null!;

    public string ReferenceDocType { get; set; } = null!;

    public int Quantity { get; set; }

    public int Balance { get; set; }

    public decimal UnitCost { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ReferenceId { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Bin Bin { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
