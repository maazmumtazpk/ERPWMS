using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ProductPricingHistory
{
    public int PriceHistoryId { get; set; }

    public int ProductId { get; set; }

    public decimal OldPrice { get; set; }

    public decimal NewPrice { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public string? Reason { get; set; }

    public int ChangedBy { get; set; }

    public virtual User ChangedByNavigation { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
