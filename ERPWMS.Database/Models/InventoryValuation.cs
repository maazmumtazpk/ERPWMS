using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class InventoryValuation
{
    public int ValuationId { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public DateOnly ValuationDate { get; set; }

    public string Method { get; set; } = null!;

    public int TotalQuantity { get; set; }

    public decimal TotalValue { get; set; }

    public decimal UnitCost { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
